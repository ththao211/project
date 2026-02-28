using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ProjectType { get; set; } = string.Empty;
        public string GuidelineUrl { get; set; } = string.Empty;

        // --- KHÓA NGOẠI (FOREIGN KEY) ---
        public Guid ManagerID { get; set; }
        [ForeignKey("ManagerID")]
        public User? Manager { get; set; }

        // 1 Project có nhiều Tasks
        public ICollection<LabelingTask>? Tasks { get; set; }
    }
}