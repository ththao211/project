using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP_BE.Models
{
    public class TaskItemDetail
    {
        [Key]
        public int IDDetail { get; set; }

        public string AnnotationData { get; set; } = string.Empty; // Chuỗi JSON chứa tọa độ/kết quả
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string Field { get; set; } = string.Empty;

        // Chi tiết này của TaskItem nào
        public int TaskItemID { get; set; }
        [ForeignKey("TaskItemID")]
        public TaskItem? TaskItem { get; set; }
    }
}