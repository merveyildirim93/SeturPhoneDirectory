using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;
using PhoneDirectory.ReportService.Controllers;
using PhoneDirectory.ReportService.Tests.TestHelpers;
using Xunit;

namespace PhoneDirectory.ReportService.Tests.Services
{
    public class ReportServiceTests
    {
        [Fact]
        public async Task Create_should_insert_preparing_report_and_publish_message()
        {
            var (ctx, conn) = SqliteInMemory.CreateReportContext();
            try
            {
                var pub = new FakePublisher();
                var controller = new ReportController(ctx, pub);

                var result = await controller.Create("Kadıköy");
                result.Should().BeOfType<AcceptedResult>();

                // DB kaydı oluştu mu?
                var rr = await ctx.ReportRequests.SingleAsync();
                rr.Status.Should().Be(ReportStatus.Preparing);
                rr.RequestedAt.Should().NotBe(default);

                // Mesaj yayınlandı mı?
                pub.LastMessage.Should().NotBeNull();
            }
            finally { conn.Dispose(); }
        }

        [Fact]
        public async Task List_should_return_reports_ordered_desc_by_requestedAt()
        {
            var (ctx, conn) = SqliteInMemory.CreateReportContext();
            try
            {
                // Arrange: iki rapor
                var r1 = new ReportRequest { RequestedAt = DateTime.UtcNow.AddMinutes(-5), Status = ReportStatus.Preparing };
                var r2 = new ReportRequest { RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed };
                await ctx.ReportRequests.AddRangeAsync(r1, r2);
                await ctx.SaveChangesAsync();

                var controller = new ReportController(ctx, new FakePublisher());

                // Act
                var res = await controller.List() as OkObjectResult;
                res.Should().NotBeNull();
                var list = (IEnumerable<object>)res!.Value!;

                // Assert: ilk eleman r2 olmalı (daha yeni)
                list.Should().HaveCount(2);
                var firstId = (Guid)list.First()!.GetType().GetProperty("id")!.GetValue(list.First())!;
                firstId.Should().Be(r2.Id);
            }
            finally { conn.Dispose(); }
        }

        [Fact]
        public async Task Detail_should_return_200_with_details_when_exists()
        {
            var (ctx, conn) = SqliteInMemory.CreateReportContext();
            try
            {
                var r = new ReportRequest { RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed };
                var d = new ReportDetail { ReportRequestId = r.Id, Location = "Kadıköy", PersonCount = 2, PhoneNumberCount = 3 };
                await ctx.ReportRequests.AddAsync(r);
                await ctx.ReportDetails.AddAsync(d);
                await ctx.SaveChangesAsync();

                var controller = new ReportController(ctx, new FakePublisher());

                var res = await controller.Detail(r.Id) as OkObjectResult;
                res.Should().NotBeNull();

                // anonymous object'ten alanları reflection ile çekiyoruz
                dynamic body = res!.Value!;
                ((Guid)body.id).Should().Be(r.Id);
                ((int)body.status).Should().Be((int)ReportStatus.Completed);

                // details içinde 1 kayıt ve doğru değerler
                var details = ((IEnumerable<object>)body.details).ToList();
                details.Should().HaveCount(1);

                var detail = details[0];
                var loc = (string)detail.GetType().GetProperty("location")!.GetValue(detail)!;
                var pc = (int)detail.GetType().GetProperty("personCount")!.GetValue(detail)!;
                var phc = (int)detail.GetType().GetProperty("phoneNumberCount")!.GetValue(detail)!;

                loc.Should().Be("Kadıköy");
                pc.Should().Be(2);
                phc.Should().Be(3);
            }
            finally { conn.Dispose(); }
        }

        [Fact]
        public async Task Detail_should_return_404_when_not_found()
        {
            var (ctx, conn) = SqliteInMemory.CreateReportContext();
            try
            {
                var controller = new ReportController(ctx, new FakePublisher());
                var res = await controller.Detail(Guid.NewGuid());
                res.Should().BeOfType<NotFoundResult>();
            }
            finally { conn.Dispose(); }
        }
    }
}
