using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        var user = _context.Users
            .FirstOrDefault(u => u.UserName == dto.Username);

        if (user == null)
            return Unauthorized("Username not found");

        if (!VerifyPassword(dto.Password, user.Password))
            return Unauthorized("Wrong password");

        if (!user.IsActive)
            return Unauthorized("Account is inactive");
        

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            token,
            role = user.Role,
            userId = user.Id
        });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
       

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
        // Nếu bạn đã hash thì thay bằng logic so sánh hash
        return inputPassword == storedPassword;
    }
}