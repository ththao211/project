using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using SWP_BE.Services;
using System.Security.Claims;

namespace SWP_BE.Controllers
{
    [Route("api/reviewer")]
    [ApiController]
    [Authorize(Roles = "Reviewer")]
    public class ReviewerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public ReviewerController(
            AppDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // ==============================
        // Pending Review
        // ==============================
        [HttpGet("tasks/pending")]
        public async Task<IActionResult> GetPendingTasks()
        {
            var reviewerId = GetCurrentUserId();

            var tasks = await _context.Tasks
                .Where(t => t.ReviewerID == reviewerId &&
                            t.Status == "PendingReview")
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskName,
                    t.ProjectID,
                    t.Deadline,
                    t.CurrentRound,
                    t.RejectCount
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // ==============================
        // Task Detail
        // ==============================
        [HttpGet("tasks/{taskId}")]
        public async Task<IActionResult> GetTaskDetail(Guid taskId)
        {
            var reviewerId = GetCurrentUserId();

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.TaskItems)
                    .ThenInclude(i => i.DataItem)
                .Include(t => t.TaskItems)
                    .ThenInclude(i => i.TaskItemDetails)
                .FirstOrDefaultAsync(t =>
                    t.TaskID == taskId &&
                    t.ReviewerID == reviewerId);

            if (task == null)
                return NotFound();

            if (task.Status != "PendingReview")
                return BadRequest("Task chưa sẵn sàng review.");

            return Ok(new
            {
                task.TaskID,
                task.TaskName,
                task.ProjectID,
                ProjectName = task.Project.ProjectName,
                task.Status,
                task.CurrentRound,
                task.RejectCount,
                task.TaskItems
            });
        }

        // ==============================
        // Approve
        // ==============================
        [HttpPost("tasks/{taskId}/approve")]
        public async Task<IActionResult> Approve(Guid taskId)
        {
            var reviewerId = GetCurrentUserId();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t =>
                    t.TaskID == taskId &&
                    t.ReviewerID == reviewerId);

            if (task == null)
                return NotFound();

            if (task.Status != "PendingReview")
                return BadRequest("Task không ở trạng thái PendingReview.");

            task.Status = "Approved";
            task.CompletedAt = DateTime.UtcNow;

            if (task.AnnotatorID.HasValue)
            {
                _context.ReputationLogs.Add(new ReputationLog
                {
                    UserID = task.AnnotatorID.Value,
                    ScoreChange = 10,
                    Reason = "Task Approved",
                    TaskID = task.TaskID
                });

                await _notificationService.NotifyTaskApproved(
                    task.AnnotatorID.Value,
                    task.TaskName
                );
            }

            await _context.SaveChangesAsync();

            return Ok("Task Approved");
        }

        // ==============================
        // Reject(3 lần )
        // ==============================
        [HttpPost("tasks/{taskId}/reject")]
        public async Task<IActionResult> Reject(Guid taskId, [FromBody] string reason)
        {
            var reviewerId = GetCurrentUserId();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t =>
                    t.TaskID == taskId &&
                    t.ReviewerID == reviewerId);

            if (task == null)
                return NotFound();

            if (task.Status != "PendingReview")
                return BadRequest("Task không ở trạng thái PendingReview.");

            task.RejectCount++;
            task.CurrentRound++;

            if (task.RejectCount >= 3)
                task.Status = "Failed";
            else
                task.Status = "PendingRework";

            if (task.AnnotatorID.HasValue)
            {
                _context.ReputationLogs.Add(new ReputationLog
                {
                    UserID = task.AnnotatorID.Value,
                    ScoreChange = -5,
                    Reason = reason,
                    TaskID = task.TaskID
                });

                await _notificationService.NotifyTaskRejected(
                    task.AnnotatorID.Value,
                    task.TaskName,
                    reason
                );
            }

            await _context.SaveChangesAsync();

            return Ok("Task Rejected");
        }

        // ==============================
        // Feedback
        // ==============================
        [HttpPost("tasks/{taskId}/feedback")]
        public async Task<IActionResult> Feedback(Guid taskId, FeedbackDTO dto)
        {
            var reviewerId = GetCurrentUserId();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t =>
                    t.TaskID == taskId &&
                    t.ReviewerID == reviewerId);

            if (task == null)
                return NotFound();

            var history = new ReviewHistory
            {
                TaskID = taskId,
                ReviewerID = reviewerId,
                FinalResult = "Feedback",
                Field = "General"
            };

            _context.ReviewHistories.Add(history);
            await _context.SaveChangesAsync();

            _context.ReviewComments.Add(new ReviewComment
            {
                HistoryID = history.HistoryID,
                Comment = dto.Comment,
                ErrorRegion = dto.ErrorRegion ?? ""
            });

            if (task.AnnotatorID.HasValue)
            {
                await _notificationService.NotifyFeedback(
                    task.AnnotatorID.Value,
                    task.TaskName
                );
            }

            await _context.SaveChangesAsync();

            return Ok("Feedback Sent");
        }

        // ==============================
        // Notifications
        // ==============================
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetCurrentUserId();

            var notifications = await _context.SystemLogs
                .Where(n => n.UserID == userId &&
                            n.EntityType == "Task")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        // ==============================
        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}