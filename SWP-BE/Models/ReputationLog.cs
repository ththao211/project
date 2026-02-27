using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class ReputationLog
    {
        [Key]
        public int ReputationLogID { get; set; }

        public int ScoreChange { get; set; } // Điểm cộng hoặc trừ
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Trỏ về User bị trừ/cộng điểm
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User? User { get; set; }

        // Liên quan đến Task nào
        public int? TaskID { get; set; }
        [ForeignKey("TaskID")]
        public LabelingTask? Task { get; set; }
    }
}