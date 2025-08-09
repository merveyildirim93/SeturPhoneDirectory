using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PhoneDirectory.ContactService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectory.ContactService.Tests.TestHelpers
{
    public static class SqliteInMemory
    {
        public static (ContactDbContext ctx, SqliteConnection conn) CreateContactContext()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();

            var options = new DbContextOptionsBuilder<ContactDbContext>()
                .UseSqlite(conn)
                .Options;

            var ctx = new ContactDbContext(options);
            ctx.Database.EnsureCreated();
            return (ctx, conn);
        }
    }
}
