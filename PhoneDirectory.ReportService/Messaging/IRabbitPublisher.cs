namespace PhoneDirectory.ReportService.Messaging
{
    public interface IRabbitPublisher
    {
        void Publish(object message);
    }
}
