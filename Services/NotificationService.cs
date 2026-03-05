using SWP_BE.Data;
using SWP_BE.Models;

namespace SWP_BE.Services
{
    public interface INotificationService
    {
        System.Threading.Tasks.Task NotifyTaskAssigned(Guid userId, string taskName, string projectName, DateTime deadline);
        System.Threading.Tasks.Task NotifyTaskRejected(Guid userId, string taskName, string reason);
        System.Threading.Tasks.Task NotifyTaskApproved(Guid userId, string taskName);
        System.Threading.Tasks.Task NotifyFeedback(Guid userId, string taskName);
    }
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public NotificationService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // =============================
        // PRIVATE HELPER
        // =============================
        private async System.Threading.Tasks.Task AddSystemLog(Guid userId, string actionType)
        {
            _context.SystemLogs.Add(new SystemLog
            {
                ActionType = actionType,
                EntityType = "Task",
                TargetID = 0,
                UserID = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        // =============================
        // TASK ASSIGNED
        // =============================
        public async System.Threading.Tasks.Task NotifyTaskAssigned(
            Guid userId,
            string taskName,
            string projectName,
            DateTime deadline)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await AddSystemLog(userId, "Task Assigned");

            await _emailService.SendTaskAssignmentEmailAsync(
                user.Email,
                user.FullName,
                taskName,
                projectName,
                deadline.ToString("dd/MM/yyyy")
            );
        }

        // =============================
        // TASK REJECTED
        // =============================
        public async System.Threading.Tasks.Task NotifyTaskRejected(
            Guid userId,
            string taskName,
            string reason)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await AddSystemLog(userId, "Task Rejected");

            await _emailService.SendTaskAssignmentEmailAsync(
                user.Email,
                user.FullName,
                taskName,
                "Task bị từ chối",
                reason
            );
        }

        // =============================
        // TASK APPROVED
        // =============================
        public async System.Threading.Tasks.Task NotifyTaskApproved(Guid userId, string taskName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await AddSystemLog(userId, "Task Approved");
        }

        // =============================
        // FEEDBACK
        // =============================
        public async System.Threading.Tasks.Task NotifyFeedback(Guid userId, string taskName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await AddSystemLog(userId, "Task Feedback");
        }
    }

}
