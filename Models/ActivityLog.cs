using System.ComponentModel.DataAnnotations;

namespace SWP_BE.Models
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public Guid? PerformedBy { get; set; }
        public Guid? TargetUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}