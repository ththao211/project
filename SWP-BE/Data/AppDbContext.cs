using Microsoft.EntityFrameworkCore;
using SWP_BE.Models;

namespace SWP_BE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- NHÓM 1: CỐT LÕI ---
        public DbSet<User> Users { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LabelingTask> LabelingTasks { get; set; }

        // --- NHÓM 2: DỮ LIỆU VÀ GẮN NHÃN ---
        public DbSet<ProjectLabel> ProjectLabels { get; set; }
        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskItemDetail> TaskItemDetails { get; set; }

        // --- NHÓM 3: REVIEW VÀ HỆ THỐNG ---
        public DbSet<ReviewHistory> ReviewHistories { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<ReputationLog> ReputationLogs { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<ExportHistory> ExportHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tắt tính năng Xóa dây chuyền (Cascade Delete) cho TOÀN BỘ Database
            // Giải quyết triệt để lỗi "multiple cascade paths" của SQL Server
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}