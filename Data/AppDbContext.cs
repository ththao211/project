using Microsoft.EntityFrameworkCore;
using SWP_BE.Models;

namespace SWP_BE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LabelingTask> LabelingTasks { get; set; }
        public DbSet<ProjectLabel> ProjectLabels { get; set; }
        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskItemDetail> TaskItemDetails { get; set; }
        public DbSet<ReviewHistory> ReviewHistories { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<ReputationLog> ReputationLogs { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<ExportHistory> ExportHistories { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Chặn xóa dây chuyền toàn hệ thống
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // --- ĐÃ XÓA HASQUERYFILTER ĐỂ FIX LỖI MỐI QUAN HỆ ---

            // 2. Ràng buộc duy nhất cho UserName
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // 3. Cấu hình ActivityLog
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasOne(a => a.Performer)
                      .WithMany()
                      .HasForeignKey(a => a.PerformedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.TargetUser)
                      .WithMany()
                      .HasForeignKey(a => a.TargetUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}