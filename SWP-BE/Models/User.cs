using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models // Tên namespace giữ nguyên theo project của bạn
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
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