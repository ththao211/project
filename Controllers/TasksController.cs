using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.Models;
using System.Security.Claims;

namespace SWP_BE.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {

        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Annotator,Reviewer")]
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            IQueryable<Tasks> tasksQuery = _context.Tasks;

            tasksQuery = roleClaim switch
            {
                "Annotator" => tasksQuery.Where(t => t.AnnotatorID == userId),
                "Reviewer" => tasksQuery.Where(t => t.ReviewerID == userId),
                _ => tasksQuery.Where(t => false)
            };

            var tasks = await tasksQuery
                .OrderBy(t => t.Deadline)
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskName,
                    t.Status,
                    t.Deadline,
                    t.ProjectID,
                    t.RejectCount,
                    t.CurrentRound,
                    t.SubmissionRate,
                    t.RateComplete
                })
                .ToListAsync();

            return Ok(tasks);
        }
    }
}
