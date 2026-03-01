using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
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

        // ===== PRIMARY KEY =====
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // ===== BASIC INFO =====
            [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

            public UserRole Role { get; set; }

            public bool IsActive { get; set; } = true;
        }
        public int UserID { get; set; }

        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Expertise { get; set; }

        // ===== SYSTEM INFO =====
        public UserRole Role { get; set; } = UserRole.Annotator;

        public int Score { get; set; } = 0;

        public int CurrentTaskCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        // ===== RELATIONSHIPS =====

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