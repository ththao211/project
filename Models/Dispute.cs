using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class Dispute
    {
        [Key]
        public int DisputeID { get; set; }

        public string Reason { get; set; } = string.Empty;
        public string ManagerComment { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ResolvedAt { get; set; } // Có thể chưa giải quyết ngay nên để null (?)

        // Trỏ về Task bị tranh chấp
        public int TaskID { get; set; }
        [ForeignKey("TaskID")]
        public LabelingTask? Task { get; set; }
    }
}