namespace PhoneDirectory.ContactService.Dtos
{
    public class PersonResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Company { get; set; }
        public List<ContactInfoResponseDto> Contacts { get; set; } = new();
    }
}
