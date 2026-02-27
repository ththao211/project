using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class ProjectLabel
    {
        [Key]
        public int ProjectLabelID { get; set; }

        public string CustomName { get; set; } = string.Empty;

        // Khóa ngoại trỏ về Project
        public int ProjectID { get; set; }
        [ForeignKey("ProjectID")]
        public Project? Project { get; set; }

        // Khóa ngoại trỏ về Label
        public int LabelID { get; set; }
        [ForeignKey("LabelID")]
        public Label? Label { get; set; }
    }
}