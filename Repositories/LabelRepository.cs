using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.Models;

namespace SWP_BE.Repositories
{
    public interface ILabelRepository
    {
        Task<IEnumerable<Label>> GetLabelsAsync(string? category);
        Task<Label?> GetByIdAsync(int id);
        Task AddAsync(Label label);
        Task UpdateAsync(Label label);
        Task DeleteAsync(Label label);
        Task<bool> IsLabelUsedInProjectsAsync(int labelId);
        Task SaveChangesAsync();
    }

    public class LabelRepository : ILabelRepository
    {
        private readonly AppDbContext _context;
        public LabelRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Label>> GetLabelsAsync(string? category)
        {
            var query = _context.Labels.AsQueryable();

            // Hỗ trợ lọc theo Category nếu có truyền param lên
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(l => l.Category.Contains(category));
            }

            return await query.ToListAsync();
        }

        public async Task<Label?> GetByIdAsync(int id)
        {
            return await _context.Labels.FindAsync(id);
        }

        public async Task AddAsync(Label label) { await _context.Labels.AddAsync(label); }

        public async Task UpdateAsync(Label label) { _context.Labels.Update(label); await Task.CompletedTask; }

        public async Task DeleteAsync(Label label) { _context.Labels.Remove(label); await Task.CompletedTask; }

        // --- HÀM NÀY QUAN TRỌNG: Check xem Label có bị dính dáng tới Project nào chưa ---
        public async Task<bool> IsLabelUsedInProjectsAsync(int labelId)
        {
            return await _context.ProjectLabels.AnyAsync(pl => pl.LabelID == labelId);
        }

        public async Task SaveChangesAsync() { await _context.SaveChangesAsync(); }
    }
}