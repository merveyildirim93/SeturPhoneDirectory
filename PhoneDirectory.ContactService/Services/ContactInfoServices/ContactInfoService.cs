using Microsoft.EntityFrameworkCore;
using PhoneDirectory.ContactService.Data;
using PhoneDirectory.ContactService.Repositories;
using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Services.ContactInfoServices
{
    public class ContactInfoService : IContactInfoService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ContactDbContext _context;

        public ContactInfoService(IPersonRepository personRepository, ContactDbContext context)
        {
            _personRepository = personRepository;
            _context = context;
        }
        public async Task<ContactInformation?> AddContactInfoAsync(Guid personId, ContactInformation info)
        {
            var person = await _personRepository.GetByIdAsync(personId);

            if (person is null) return null;

            info.Id = Guid.Empty;           
            info.PersonId = person.Id;        
            _context.Entry(info).State = EntityState.Added;

            await _personRepository.SaveChangesAsync();
            return info;
        }


        public async Task<bool> RemoveContactInfoAsync(Guid personId, Guid contactInfoId)
        {
            var person = await _personRepository.GetByIdAsync(personId);
            if (person is null) return false;

            var ci = person.ContactInformations.FirstOrDefault(x => x.Id == contactInfoId);
            if (ci is null) return false;

            person.ContactInformations.Remove(ci);
            await _personRepository.SaveChangesAsync();
            return true;
        }
    }
}
