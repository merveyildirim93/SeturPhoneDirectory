using PhoneDirectory.ReportService.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectory.ReportService.Tests.TestHelpers
{
    public class FakePublisher : IRabbitPublisher
    {
        public object? LastMessage { get; private set; }
        public void Publish(object message) => LastMessage = message;
    }
}
