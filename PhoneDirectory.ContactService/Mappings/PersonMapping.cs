using PhoneDirectory.ContactService.Dtos;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;

namespace PhoneDirectory.ContactService.Mappings
{
    public static class PersonMapping
    {
        public static Person ToEntity(this PersonCreateRequestDto dto)
        {
            var person = new Person
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Company = dto.Company
            };

            if (dto.Contacts != null)
            {
                foreach (var c in dto.Contacts)
                {
                    person.ContactInformations.Add(new ContactInformation
                    {
                        Type = (ContactType)c.Type,
                        Value = c.Value
                    });
                }
            }

            return person;
        }

        public static PersonResponseDto ToResponse(this Person entity)
        {
            return new PersonResponseDto
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Company = entity.Company,
                Contacts = entity.ContactInformations?
                    .Select(ci => new ContactInfoResponseDto
                    {
                        Id = ci.Id,
                        Type = (int)ci.Type,
                        Value = ci.Value
                    }).ToList() ?? new()
            };
        }

        public static Person ToEntityForUpdate(this PersonUpdateRequestDto dto)
        {
            var person = new Person
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Company = dto.Company
            };

            if (dto.Contacts != null)
            {
                foreach (var c in dto.Contacts)
                {
                    person.ContactInformations.Add(new ContactInformation
                    {
                        Type = (ContactType)c.Type,
                        Value = c.Value
                    });
                }
            }

            return person;
        }

    }
}
