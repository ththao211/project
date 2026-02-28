using SWP_BE.Models;
using static SWP_BE.Models.User;

namespace SWP_BE.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserRole Role { get; set; }
    }
}