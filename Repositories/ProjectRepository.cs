using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.Models;

namespace SWP_BE.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllByManagerIdAsync(Guid managerId);
        Task<Project?> GetByIdAndManagerAsync(Guid projectId, Guid managerId);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task SaveChangesAsync();
        Task AddDataItemsAsync(IEnumerable<DataItem> dataItems);
        Task<List<DataItem>> GetUnassignedDataAsync(Guid projectId, int takeCount);
        Task AddTaskAsync(LabelingTask task);
        Task AddTaskItemsAsync(IEnumerable<TaskItem> taskItems);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Project>> GetAllByManagerIdAsync(Guid managerId)
        {
            return await _context.Projects
                .Include(p => p.DataItems) // Bốc kèm mảng ảnh từ DB
                .Where(p => p.ManagerID == managerId)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAndManagerAsync(Guid projectId, Guid managerId)
        {
            return await _context.Projects
                .Include(p => p.DataItems)
                .FirstOrDefaultAsync(p => p.ProjectID == projectId && p.ManagerID == managerId);
        }

        public async Task AddAsync(Project project) { await _context.Projects.AddAsync(project); }
        public async Task UpdateAsync(Project project) { _context.Projects.Update(project); await Task.CompletedTask; }
        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
        public async Task AddDataItemsAsync(IEnumerable<DataItem> dataItems) { await _context.DataItems.AddRangeAsync(dataItems); }

        public async Task<List<DataItem>> GetUnassignedDataAsync(Guid projectId, int takeCount)
        {
            // Fix lỗi so sánh int và Guid
            return await _context.DataItems
                .Where(d => d.ProjectID == projectId && d.IsAssigned == false)
                .Take(takeCount)
                .ToListAsync();
        }

        public async Task AddTaskAsync(LabelingTask task) { await _context.LabelingTasks.AddAsync(task); }
        public async Task AddTaskItemsAsync(IEnumerable<TaskItem> taskItems) { await _context.TaskItems.AddRangeAsync(taskItems); }
    }
}