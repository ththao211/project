using static SWP_BE.Models.User;

namespace SWP_BE.DTOs
{
    public class UpdateUserDTO
    {
        public string? Username { get; set; }  
        public bool? IsActive { get; set; }
        public UserRole Role { get; set; }
    }
}