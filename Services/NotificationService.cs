using SWP_BE.Data;
using SWP_BE.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

        private async System.Threading.Tasks.Task AddSystemLog(Guid userId, string actionType, Guid? targetId = null)
        {
            _context.SystemLogs.Add(new SystemLog
            {
                ActionType = actionType,
                EntityType = "Task",
                TargetID = (targetId ?? Guid.Empty).ToString(),

                UserID = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
        // 1. THÔNG BÁO GIAO TASK (Task Assigned)
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

        // ============================================================
        // 2. THÔNG BÁO TASK BỊ REJECT (Rejected)
        // ============================================================
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
                "Task bị từ chối (Rejected)",
                $"Lý do: {reason}"
            );
        }

        // ============================================================
        // 3. THÔNG BÁO TASK ĐƯỢC DUYỆT (Approved)
        // ============================================================
        public async System.Threading.Tasks.Task NotifyTaskApproved(Guid userId, string taskName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await AddSystemLog(userId, "Task Approved");
        }

        // ============================================================
        // 4. THÔNG BÁO CÓ PHẢN HỒI (Feedback)
        // ============================================================
        public async System.Threading.Tasks.Task NotifyFeedback(Guid userId, string taskName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await AddSystemLog(userId, "Task Feedback");
        }
    }
}