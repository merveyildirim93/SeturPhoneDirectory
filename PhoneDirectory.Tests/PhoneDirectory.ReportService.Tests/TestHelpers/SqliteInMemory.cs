using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PhoneDirectory.ReportService.Data;

namespace PhoneDirectory.ReportService.Tests.TestHelpers
{
    public static class SqliteInMemory
    {
        public static (ReportDbContext ctx, SqliteConnection conn) CreateReportContext()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();

            var options = new DbContextOptionsBuilder<ReportDbContext>()
                .UseSqlite(conn)
                .Options;

            var ctx = new ReportDbContext(options);
            ctx.Database.EnsureCreated();
            return (ctx, conn);
        }
    }
}
