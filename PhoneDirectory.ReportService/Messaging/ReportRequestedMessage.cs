namespace PhoneDirectory.ReportService.Messaging
{
    public class ReportRequestedMessage
    {
        public Guid ReportId { get; set; }
        public string Location { get; set; }
    }
}
