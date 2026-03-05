using Microsoft.AspNetCore.Mvc;
using SWP_BE.Data;
using SWP_BE.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ScoreController : ControllerBase
{
    private readonly AppDbContext _context;

    public ScoreController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetScores(
        [FromQuery] string? role,
        [FromQuery] Guid? userId)
    {
        var query = _context.Users
            .Where(u => u.Role == SWP_BE.Models.User.UserRole.Annotator
                     || u.Role == SWP_BE.Models.User.UserRole.Reviewer)
            .AsQueryable();

        // Filter theo role
        if (!string.IsNullOrEmpty(role))
        {
            if (Enum.TryParse<User.UserRole>(role, true, out var parsedRole))
            {
                query = query.Where(u => u.Role == parsedRole);
            }
            else
            {
                return BadRequest("Invalid role");
            }
        }

        // Filter theo userId
        if (userId.HasValue)
        {
            query = query.Where(u => u.UserID == userId.Value);
        }

        var result = await query
            .Select(u => new
            {
                u.UserID,
                u.UserName,
                u.FullName,
                Role = u.Role.ToString(),
                CurrentScore = u.Score,
                TotalScoreChange = u.ReputationLogs
                    .Sum(r => (int?)r.ScoreChange) ?? 0,
                TaskCount = u.Role == SWP_BE.Models.User.UserRole.Annotator
                    ? u.AnnotatorTasks.Count()
                    : u.ReviewerTasks.Count()
            })
            .ToListAsync();

        return Ok(result);
    }
}