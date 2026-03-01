using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SWP_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == dto.Username);

            if (user == null || !VerifyPassword(dto.Password, user.Password))
                return Unauthorized("Invalid username or password");

            if (!user.IsActive)
                return Unauthorized("Account is inactive");

            var token = GenerateJwtToken(user);

            return Ok(new { token, role = (int)user.Role, userId = user.UserID });
        }

        private string GenerateJwtToken(User user)
        {
            // Ép kiểu (int) để dùng switch case chuẩn xác
            string roleName = ((int)user.Role) switch
            {
                1 => "Admin",
                2 => "Manager",
                3 => "Annotator",
                4 => "Reviewer",
            };

            var claims = new[]
            {
                new Claim("id", user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
            }
            catch
            {
                return inputPassword == storedPassword;
            }
        }
    }
}