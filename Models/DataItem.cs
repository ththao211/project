using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class DataItem
    {
        [Key]
        public Guid DataID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public bool IsAssigned { get; set; } = false;
        public Guid ProjectID { get; set; }
        [ForeignKey("ProjectID")]
        public Project? Project { get; set; }
    }
}