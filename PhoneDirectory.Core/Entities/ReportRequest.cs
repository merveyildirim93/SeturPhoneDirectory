using PhoneDirectory.Core.Enums;

namespace PhoneDirectory.Core.Entities
{
    public class ReportRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime RequestedAt { get; set; } 
        public ReportStatus Status { get; set; }

        public ICollection<ReportDetail> ReportDetails { get; set; } = new List<ReportDetail>();
    }
}
