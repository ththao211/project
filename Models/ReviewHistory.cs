using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class ReviewHistory
    {
        [Key]
        public int HistoryID { get; set; }

        public int IDDetail { get; set; } // Liên kết mềm với TaskItemDetail
        public DateTime ReviewAt { get; set; } = DateTime.Now;
        public string FinalResult { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;

        // Trỏ về Task
        public int TaskID { get; set; }
        [ForeignKey("TaskID")]
        public LabelingTask? Task { get; set; }

        // Trỏ về người duyệt (Reviewer)
        public Guid ReviewerID { get; set; }
        [ForeignKey("ReviewerID")]
        public User? Reviewer { get; set; }
    }
}