using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using static SWP_BE.Models.User;

namespace SWP_BE.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context) { _context = context; }


        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (dto.Role < 1 || dto.Role > 4)
                return BadRequest("Role không hợp lệ! (1:Admin, 2:Manager, 3:Annotator, 4:Reviewer)");

            if (await _context.Users.AnyAsync(x => x.UserName == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                UserID = Guid.NewGuid(),
                UserName = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                Expertise = dto.Expertise,
                Role = (UserRole)dto.Role,
                IsActive = true,
                Score = 100
            };

            _context.Users.Add(user);
            await LogActivity("Create User", user.UserID);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully", userId = user.UserID });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _context.Users.Where(u => u.IsActive).ToListAsync());
        }

        [Authorize]
        [HttpPut("update-user-info")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var currentUserId = User.FindFirst("id")?.Value;
            var currentRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (user.UserID.ToString() != currentUserId && currentRole != "Admin")
                return Forbid("Access denied.");

            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // CHẶN ROLE KHI UPDATE: Chỉ Admin được đổi và phải đổi đúng số
            if (dto.Role != 0 && currentRole == "Admin")
            {
                if (dto.Role < 1 || dto.Role > 4) return BadRequest("Role không hợp lệ!");
                user.Role = (UserRole)dto.Role;
            }

            await LogActivity("Update User", user.UserID);
            await _context.SaveChangesAsync();
            return Ok("Updated");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            await LogActivity("Deactivate User", user.UserID);
            await _context.SaveChangesAsync();
            return Ok("Deactivated");
        }

        private async Task LogActivity(string action, Guid targetUserId)
        {
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
        }
    }
}