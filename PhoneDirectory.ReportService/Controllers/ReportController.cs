using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;
using PhoneDirectory.ReportService.Data;
using PhoneDirectory.ReportService.Messaging;

namespace PhoneDirectory.ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportDbContext _db;
        private readonly ReportPublisher _publisher;

        public ReportController(ReportDbContext db, ReportPublisher publisher)
        {
            _db = db; _publisher = publisher;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("location is required");

            var report = new ReportRequest
            {
                RequestedAt = DateTime.UtcNow,
                Status = ReportStatus.Preparing
            };

            await _db.ReportRequests.AddAsync(report);
            await _db.SaveChangesAsync();

            _publisher.Publish(new ReportRequestedMessage
            {
                ReportId = report.Id,
                Location = location
            });

            return Accepted(new { id = report.Id, status = report.Status, location });
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _db.ReportRequests
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new
                {
                    r.Id,
                    r.RequestedAt,
                    r.Status
                }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var r = await _db.ReportRequests
                .Include(x => x.ReportDetails)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (r == null) return NotFound();

            return Ok(new
            {
                r.Id,
                r.RequestedAt,
                r.Status,
                Details = r.ReportDetails.Select(d => new
                {
                    d.Location,
                    d.PersonCount,
                    d.PhoneNumberCount
                })
            });
        }
    }
}

