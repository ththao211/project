using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class ExportHistory
    {
        [Key]
        public int ExportID { get; set; }
        public string Format { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ProjectID { get; set; }
        [ForeignKey("ProjectID")]
        public Project? Project { get; set; }

        public Guid ManagerID { get; set; }
        [ForeignKey("ManagerID")]
        public User? Manager { get; set; }
    }
}