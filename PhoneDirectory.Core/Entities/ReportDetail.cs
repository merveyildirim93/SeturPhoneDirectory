namespace PhoneDirectory.Core.Entities
{
    public class ReportDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Location { get; set; }

        public int PersonCount { get; set; }

        public int PhoneNumberCount { get; set; }

        public Guid ReportRequestId { get; set; }
        public ReportRequest ReportRequest { get; set; }
    }
}
