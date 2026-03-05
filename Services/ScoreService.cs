using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;

namespace SWP_BE.Services
{
    public interface IScoreService
    {
        Task<List<UserScoreResponseDto>> GetAllScores();
        Task<List<UserScoreResponseDto>> GetByRole(User.UserRole role);
        Task<UserScoreResponseDto?> GetByUserId(Guid userId);
    }

    public class ScoreService : IScoreService
    {
        private readonly AppDbContext _context;

        public ScoreService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserScoreResponseDto>> GetAllScores()
        {
            return await _context.Users
                .Where(u => u.Role == User.UserRole.Annotator
                         || u.Role == User.UserRole.Reviewer)
                .Select(u => new UserScoreResponseDto
                {
                    UserId = u.UserID,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    Role = u.Role,

                    CurrentScore = u.Score,

                    TotalScoreChange = u.ReputationLogs
                        .Sum(r => (int?)r.ScoreChange) ?? 0,

                    TaskCount = u.Role == User.UserRole.Annotator
                        ? u.AnnotatorTasks.Count()
                        : u.ReviewerTasks.Count()
                })
                .ToListAsync();
        }

        public async Task<List<UserScoreResponseDto>> GetByRole(User.UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .Select(u => new UserScoreResponseDto
                {
                    UserId = u.UserID,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    Role = u.Role,

                    CurrentScore = u.Score,

                    TotalScoreChange = u.ReputationLogs
                        .Sum(r => (int?)r.ScoreChange) ?? 0,

                    TaskCount = role == User.UserRole.Annotator
                        ? u.AnnotatorTasks.Count()
                        : u.ReviewerTasks.Count()
                })
                .ToListAsync();
        }

        public async Task<UserScoreResponseDto?> GetByUserId(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.ReputationLogs)
                .Include(u => u.AnnotatorTasks)
                .Include(u => u.ReviewerTasks)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null) return null;

            return new UserScoreResponseDto
            {
                UserId = user.UserID,
                UserName = user.UserName,
                FullName = user.FullName,
                Role = user.Role,

                CurrentScore = user.Score,

                TotalScoreChange = user.ReputationLogs
                    .Sum(r => r.ScoreChange),

                TaskCount = user.Role == User.UserRole.Annotator
                    ? user.AnnotatorTasks.Count
                    : user.ReviewerTasks.Count
            };
        }
    }
}
