namespace PhoneDirectory.ContactService.Dtos
{
    public class PersonUpdateRequestDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Company { get; set; }
        public List<ContactInfoRequestDto> Contacts { get; set; } = new();
    }
}
