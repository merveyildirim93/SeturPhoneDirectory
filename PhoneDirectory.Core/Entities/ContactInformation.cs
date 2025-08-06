using PhoneDirectory.Core.Enums;

namespace PhoneDirectory.Core.Entities
{
    public class ContactInformation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ContactType Type { get; set; }
        public string Value { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
    }
}
