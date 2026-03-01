using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using System.Security.Claims;

namespace SWP_BE.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context) { _context = context; }

        //[Authorize(Roles = "Admin")]
        [HttpPost("CREATE USER")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == dto.Username))
                return BadRequest("Username already exists");

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                UserID = Guid.NewGuid(),
                UserName = dto.Username,
                Password = dto.Password, // Lưu ý: Nên Hash password ở đây
                FullName = dto.FullName,
                Email = dto.Email,
                Expertise = dto.Expertise,
                Role = dto.Role,
                IsActive = true,
                Score = 100
            };

            _context.Users.Add(user);
            await LogActivity("Create User", user.UserID);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully", userId = user.UserID });
        }

        [HttpGet("Get all User")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Vì đã bỏ QueryFilter, ta chủ động lọc Active ở đây nếu cần
            return Ok(await _context.Users.Where(u => u.IsActive).ToListAsync());
        }

        [Authorize]
        [HttpPut("UPDATE USER INFO")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var currentUserName = User.Identity?.Name;
            var currentRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (user.UserName != currentUserName && currentRole != "Admin")
                return Forbid("You can only update your own account.");

            // Cập nhật các trường thông tin
            if (!string.IsNullOrEmpty(dto.Username)) user.UserName = dto.Username;
            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Expertise)) user.Expertise = dto.Expertise;

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
            if (dto.Role != 0) user.Role = dto.Role;

            await LogActivity("Update User", user.UserID);
            await _context.SaveChangesAsync();
            return Ok("User updated successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DELETE-USER/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.UserName == User.Identity?.Name)
                return BadRequest("You cannot delete yourself.");

            user.IsActive = false;
            await LogActivity("Deactivate User", user.UserID);
            await _context.SaveChangesAsync();
            return Ok("User deactivated");
        }

        private Task LogActivity(string action, Guid targetUserId)
        {
            // Sửa lại để lấy đúng claim "id" đã cài đặt
            var currentUserIdStr = User.FindFirst("id")?.Value;

            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = action,
                TargetUserId = targetUserId,
                PerformedBy = currentUserIdStr != null ? Guid.Parse(currentUserIdStr) : null,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);
            return Task.CompletedTask;
        }
    }
}