using Microsoft.EntityFrameworkCore;
using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ReportService.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) { }

        public DbSet<ReportRequest> ReportRequests => Set<ReportRequest>();
        public DbSet<ReportDetail> ReportDetails => Set<ReportDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportRequest>()
                .HasMany(r => r.ReportDetails)
                .WithOne(d => d.ReportRequest)
                .HasForeignKey(d => d.ReportRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
