using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SWP_BE.Models // Tên namespace giữ nguyên theo project của bạn
{
    public class User
    {
        public enum UserRole
        {
            Admin = 1,
            Manager = 2,
            Annotator = 3,
            Reviewer = 4
        }
        public class Users
        {
            public Guid Id { get; set; }

            [Required]
            public string Username { get; set; } = null!;

            [Required]
            public string PasswordHash { get; set; } = null!;

            public UserRole Role { get; set; }

            public bool IsActive { get; set; } = true;
        }
        public int UserID { get; set; }

        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public string? Expertise { get; set; }
        public int Score { get; set; }
        public int CurrentTaskCount { get; set; }
        public bool IsActive { get; set; } = true;

        // --- MỐI QUAN HỆ (NAVIGATION PROPERTIES) ---
        // 1 User làm Manager của nhiều Project
        [InverseProperty("Manager")]
        public ICollection<Project>? ManagedProjects { get; set; }

        // 1 User làm Annotator của nhiều Task
        [InverseProperty("Annotator")]
        public ICollection<LabelingTask>? AnnotatorTasks { get; set; }

        // 1 User làm Reviewer của nhiều Task
        [InverseProperty("Reviewer")]
        public ICollection<LabelingTask>? ReviewerTasks { get; set; }
    }
}