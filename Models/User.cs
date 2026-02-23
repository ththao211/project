using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Email { get; set; }

        public string Expertise { get; set; }

        public int Score { get; set; } = 100;

        public int CurrentTaskCount { get; set; }

        public bool IsActive { get; set; } = true;

        public int ConsecutiveFailCount { get; set; }

        public int ConsecutiveReviewCount { get; set; }

        public int SpamCount { get; set; }

        public int TotalTaskCompleted { get; set; }
    }
}