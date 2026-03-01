using static SWP_BE.Models.User;

namespace SWP_BE.DTOs
{
    public class UpdateUserDTO
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public String? Expertise { get; set; }
        public int? Score { get; set; }
        public int? CurrentTaskCount { get; set; }
        public bool? IsActive { get; set; }
        public UserRole Role { get; set; }
    }
}