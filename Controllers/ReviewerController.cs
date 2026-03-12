using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using SWP_BE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SWP_BE.Controllers
{
    [Route("api/reviewer")]
    [ApiController]
    [Authorize(Roles = "Reviewer")]
    [Tags("Reviewer Task Management")]
    public class ReviewerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ReputationService _reputationService;
        private readonly IProgressService _progressService;

        public ReviewerController(
            AppDbContext context,
            ReputationService reputationService,
            INotificationService notificationService,
            IProgressService progressService)
        {
            _context = context;
            _reputationService = reputationService;
            _notificationService = notificationService;
            _progressService = progressService;
        }

        // ============================================================
        // 1. LẤY DANH SÁCH TASK 
        // ============================================================
        [HttpGet("tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetReviewerTasks([FromQuery] string? status)
        {
            var reviewerId = GetCurrentUserId();
            var query = _context.Tasks.Where(t => t.ReviewerID == reviewerId);
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<SWP_BE.Models.Task.TaskStatus>(status, true, out var taskStatus))
                {
                    query = query.Where(t => t.Status == taskStatus);
                }
                else
                {
                    return BadRequest($"Trạng thái '{status}' không hợp lệ.");
                }
            }
            var tasks = await query
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskName,
                    t.ProjectID,
                    Status = t.Status.ToString(),
                    t.Deadline,
                    t.CurrentRound // Đã xóa RejectCount
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // ============================================================
        // 2. XEM CHI TIẾT TASK 
        // ============================================================
        [HttpGet("tasks/{taskId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

            if (task == null) return NotFound("Không tìm thấy Task.");

            if (task.Status != SWP_BE.Models.Task.TaskStatus.PendingReview)
                return BadRequest("Task không ở trạng thái chờ duyệt.");

            return Ok(new
            {
                task.TaskID,
                task.TaskName,
                ProjectName = task.Project?.ProjectName,
                Status = task.Status.ToString(),
                task.CurrentRound, // Đã xóa RejectCount
                Items = task.TaskItems.Select(i => new {
                    i.ItemID,
                    i.DataItem.FilePath,
                    i.DataItem.FileName,
                    Annotations = i.TaskItemDetails.Select(d => new {
                        d.IDDetail,
                        d.AnnotationData,
                        d.Content,
                        d.Field,
                        d.IsApproved
                    })
                })
            });
        }

        // ============================================================
        // 3. CHECK ĐÚNG/SAI TỪNG DATA 
        // ============================================================
        [HttpPatch("item-detail/{id}/check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReviewItemDetail(int id, [FromQuery] bool isApproved)
        {
            var detail = await _context.TaskItemDetails.FindAsync(id);
            if (detail == null) return NotFound();

            detail.IsApproved = isApproved;

            await _context.SaveChangesAsync();

            var taskId = await _context.TaskItems
                .Where(i => i.ItemID == detail.TaskItemID)
                .Select(i => i.TaskID)
                .FirstOrDefaultAsync();

            await _progressService.UpdateTaskAndProject(taskId);

            return Ok(new { message = isApproved ? "Đã đánh dấu ĐÚNG" : "Đã đánh dấu SAI" });
        }

        // ============================================================
        // 4. APPROVE (Duyệt toàn bộ Task)
        // ============================================================
        [HttpPost("tasks/{taskId}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Approve(Guid taskId)
        {
            var reviewerId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskID == taskId && t.ReviewerID == reviewerId);

            if (task == null || task.Status != SWP_BE.Models.Task.TaskStatus.PendingReview)
                return BadRequest("Thao tác không hợp lệ.");

            task.Status = SWP_BE.Models.Task.TaskStatus.Approved;
            task.CompletedAt = DateTime.UtcNow;

            if (task.AnnotatorID.HasValue)
            {
                await _reputationService.HandleTaskCompletionAsync(task.AnnotatorID.Value, task);
                await _notificationService.NotifyTaskApproved(task.AnnotatorID.Value, task.TaskName);
            }

            await _context.SaveChangesAsync();
            await _progressService.UpdateTaskAndProject(task.TaskID);
            return Ok("Task Approved và đã cập nhật điểm tín nhiệm.");
        }

        // ============================================================
        // 5. REJECT (Từ chối Task)
        // ============================================================
        [HttpPost("tasks/{taskId}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reject(Guid taskId, [FromBody] FeedbackDTO feedback)
        {
            var reviewerId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskID == taskId && t.ReviewerID == reviewerId);

            if (task == null || task.Status != SWP_BE.Models.Task.TaskStatus.PendingReview)
                return BadRequest("Task không hợp lệ.");

            if (string.IsNullOrWhiteSpace(feedback.Comment))
                return BadRequest("Vui lòng nhập lý do từ chối.");

            // KIỂM TRA LOGIC FAIL: Dựa vào CurrentRound
            // Nếu đây là lần nộp thứ 4 (tức là đã sửa 3 lần rồi) mà vẫn bị Reject -> Cho Rớt luôn
            if (task.CurrentRound == 4)
            {
                task.Status = SWP_BE.Models.Task.TaskStatus.Fail;
                task.CompletedAt = DateTime.UtcNow;
                if (task.AnnotatorID.HasValue)
                {
                    // LƯU Ý: ReputationService.HandleTaskCompletionAsync cần xử lý case task.Status == Fail để trừ 20 điểm
                    await _reputationService.HandleTaskCompletionAsync(task.AnnotatorID.Value, task);
                    await _notificationService.NotifyTaskRejected(task.AnnotatorID.Value, task.TaskName, "Task bị FAIL do vượt quá 3 lần sửa.");
                }
            }
            else
            {
                // Nếu chưa tới vòng 4 thì trả về InProgress cho Annotator sửa tiếp
                task.Status = SWP_BE.Models.Task.TaskStatus.InProgress;

                if (task.AnnotatorID.HasValue)
                {
                    await _notificationService.NotifyTaskRejected(task.AnnotatorID.Value, task.TaskName, feedback.Comment);
                }
            }

            await _context.SaveChangesAsync();
            await _progressService.UpdateTaskAndProject(task.TaskID);

            // Sửa lại câu thông báo trả về
            return Ok(task.Status == SWP_BE.Models.Task.TaskStatus.Fail ? "Task đã bị đánh FAIL" : $"Task bị Reject, chuyển về InProgress. (Vòng nộp tiếp theo: {task.CurrentRound + 1})");
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                throw new UnauthorizedAccessException("Phiên đăng nhập không hợp lệ hoặc thiếu ID.");
            }
            return userId;
        }
    }
}