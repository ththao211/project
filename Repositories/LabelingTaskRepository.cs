using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.Models;

namespace SWP_BE.Repositories
{
    public interface ILabelingTaskRepository
    {
        Task<IEnumerable<DataItem>> GetUnassignedDataByProjectIdAsync(Guid projectId);
        Task<List<DataItem>> GetDataItemsByIdsAsync(Guid projectId, List<Guid> dataIds);
        Task<LabelingTask?> GetTaskByIdAsync(Guid taskId);
        Task<IEnumerable<LabelingTask>> GetTasksByProjectIdAsync(Guid projectId);
        Task CreateTaskWithItemsAsync(LabelingTask task, List<TaskItem> taskItems, List<DataItem> updatedDataItems);
        Task UpdateTaskAsync(LabelingTask task);
        Task SaveChangesAsync();
    }

    public class LabelingTaskRepository : ILabelingTaskRepository
    {
        private readonly AppDbContext _context;
        public LabelingTaskRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<DataItem>> GetUnassignedDataByProjectIdAsync(Guid projectId)
        {
            return await _context.DataItems
                .Where(d => d.ProjectID == projectId && d.IsAssigned == false)
                .ToListAsync();
        }

        public async Task<List<DataItem>> GetDataItemsByIdsAsync(Guid projectId, List<Guid> dataIds)
        {
            return await _context.DataItems
                .Where(d => d.ProjectID == projectId && dataIds.Contains(d.DataID))
                .ToListAsync();
        }

        public async Task<LabelingTask?> GetTaskByIdAsync(Guid taskId)
        {
            return await _context.LabelingTasks.FindAsync(taskId);
        }

        public async Task<IEnumerable<LabelingTask>> GetTasksByProjectIdAsync(Guid projectId)
        {
            // Include thêm TaskItems để đếm số lượng file trong Task cho API 4
            return await _context.LabelingTasks
                .Include(t => t.TaskItems)
                .Where(t => t.ProjectID == projectId)
                .ToListAsync();
        }

        // Dùng Transaction để đảm bảo 3 bước của API 2 (Tạo Task, Tạo TaskItem, Update DataItem) thành công cùng lúc
        public async Task CreateTaskWithItemsAsync(LabelingTask task, List<TaskItem> taskItems, List<DataItem> updatedDataItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.LabelingTasks.AddAsync(task);
                await _context.TaskItems.AddRangeAsync(taskItems);
                _context.DataItems.UpdateRange(updatedDataItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateTaskAsync(LabelingTask task)
        {
            _context.LabelingTasks.Update(task);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}