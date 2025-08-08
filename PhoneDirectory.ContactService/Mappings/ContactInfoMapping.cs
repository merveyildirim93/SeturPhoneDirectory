using PhoneDirectory.ContactService.Dtos;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;

namespace PhoneDirectory.ContactService.Mappings
{
    public static class ContactInfoMapping
    {
        public static ContactInformation ToEntity(this ContactInfoRequestDto dto)
            => new ContactInformation
            {
                Type = (ContactType)dto.Type,
                Value = dto.Value
            };
    }
}
