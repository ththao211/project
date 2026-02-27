using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    [Table("Tasks")] // Ép Entity Framework tạo bảng tên là "Tasks" trong SQL thay vì "LabelingTasks"
    public class LabelingTask
    {
        [Key]
        public int TaskID { get; set; }

        public string TaskName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RejectCount { get; set; }
        public double RateComplete { get; set; }
        public DateTime Deadline { get; set; }
        public int CurrentRound { get; set; }
        public double SubmissionRate { get; set; }

        // --- KHÓA NGOẠI 1: Trỏ về Project ---
        public int ProjectID { get; set; }
        [ForeignKey("ProjectID")]
        public Project? Project { get; set; }

        // --- KHÓA NGOẠI 2: Trỏ về User (Người gắn nhãn) ---
        public int AnnotatorID { get; set; }
        [ForeignKey("AnnotatorID")]
        public User? Annotator { get; set; }

        // --- KHÓA NGOẠI 3: Trỏ về User (Người duyệt) ---
        public int? ReviewerID { get; set; }
        [ForeignKey("ReviewerID")]
        public User? Reviewer { get; set; }
    }
}