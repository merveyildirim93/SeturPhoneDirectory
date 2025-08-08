using PhoneDirectory.Core.Enums;

namespace PhoneDirectory.Core.Entities
{
    public class ReportRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public ReportStatus Status { get; set; } = ReportStatus.Preparing;

        public ICollection<ReportDetail> ReportDetails { get; set; } = new List<ReportDetail>();
    }

}
