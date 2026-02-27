using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class DataItem
    {
        [Key]
        public int DataID { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;

        // Dữ liệu này thuộc về Project nào
        public int ProjectID { get; set; }
        [ForeignKey("ProjectID")]
        public Project? Project { get; set; }
    }
}