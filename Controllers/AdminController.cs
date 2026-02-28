
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using System.Security.Cryptography;
using System.Text;
using static SWP_BE.Models.User;

namespace SWP_BE.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")] //CHỈ ADMIN
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE USER
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.Username,
                Password = HashPassword(dto.Password),
                Role = dto.Role,     
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET USER BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // UPDATE USER INFO
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Username))
                user.UserName = dto.Username;

            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;

            await _context.SaveChangesAsync();

            return Ok("User updated");
        }
        // DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted");
        }

        // UPDATE ROLE (PHÂN ROLE)
        [HttpPut("{id}/role")]
   
        public async Task<IActionResult> UpdateUserRole(Guid id, UpdateRoleDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = dto.Role;

            await _context.SaveChangesAsync();

            return Ok("Role updated");
        }
        // HASH PASSWORD (Simple SHA256)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}