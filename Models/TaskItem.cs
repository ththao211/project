using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class TaskItem
    {
        [Key]
        public int ItemID { get; set; }

        public bool IsFlagged { get; set; }

        // Thuộc Task nào
        public int TaskID { get; set; }
        [ForeignKey("TaskID")]
        public LabelingTask? Task { get; set; }

        // Dữ liệu gốc là gì
        public int DataID { get; set; }
        [ForeignKey("DataID")]
        public DataItem? DataItem { get; set; }
    }
}