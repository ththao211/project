
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using System.Data;
using System.Security.Claims;
using static SWP_BE.Models.User;

namespace SWP_BE.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
   
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }


        // CREATE USER
        [Authorize(Roles = "Admin")]
        [HttpPost("CREATE USER")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == dto.Username))
                return BadRequest("Username already exists");

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email already exists");
            if (!Enum.IsDefined(typeof(UserRole), dto.Role))
                return BadRequest("Invalid role");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.Username,
               // Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // HASH PASSWORD
                Password = dto.Password,
                FullName = dto.FullName,
                Email = dto.Email,
                Expertise = dto.Expertise,
                Role = dto.Role,

                // DEFAULT VALUES
                Score = 100,
                CurrentTaskCount = 0,
                IsActive = true
            };

            _context.Users.Add(user);

            await LogActivity("Create User", user.Id);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully",
                userId = user.Id
            });
        }
        // GET ALL USERS
        [HttpGet("Get all User")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        // UPDATE USER INFO

        [Authorize]
        [HttpPut("UPDATE USER INFO")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var currentUserName = User.Identity?.Name;
            var currentRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Chỉ Admin được quyền chỉnh user khác
            if (user.UserName != currentUserName && currentRole != "Admin")
            {
                return Forbid("You can only update your own account.");
            }

            // Admin ko thể chỉnh Admin khác
            if (currentRole == "Admin" && user.Role == UserRole.Admin && user.UserName != currentUserName)
            {
                return Forbid("Admin cannot modify another Admin.");
            }


            if (!string.IsNullOrEmpty(dto.UserName))
                user.UserName = dto.UserName;

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Expertise))
                user.Expertise = dto.Expertise;

            

            await LogActivity("Update User", user.Id);
            await _context.SaveChangesAsync();

            return Ok("User updated successfully");
        }
        
        // FR-01 DELETE USER 
        [Authorize(Roles = "Admin")]
        [HttpDelete("DELETE-USER/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            var currentUserName = User.Identity?.Name;

            // Admin không tự xoá chính mình
            if (user.UserName == currentUserName)
                return BadRequest("You cannot delete yourself.");

            // Admin không được xoá Admin khác
            if (user.Role == SWP_BE.Models.User.UserRole.Admin)
                return BadRequest("You cannot delete another Admin.");

         
            user.IsActive = false;

            await LogActivity("Deactivate User", user.Id);
            await _context.SaveChangesAsync();

            return Ok("User has been deactivated successfully.");
        }
        //  FR-02 CHANGE ROLE
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole(Guid id, UserRole newRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = newRole;

            await LogActivity("Change Role", user.Id);

            await _context.SaveChangesAsync();

            return Ok("Role Updated");
        }
        // FR-03 VIEW LOGS
        [HttpGet("View logs")]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(await _context.ActivityLogs
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync());
        }
        private Task LogActivity(string action, Guid targetUserId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = action,
                TargetUserId = targetUserId,     
                PerformedBy = currentUserId != null ? Guid.Parse(currentUserId) : targetUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);

            return Task.CompletedTask;  
        }
     }
    }