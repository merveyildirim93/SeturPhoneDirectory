using Microsoft.EntityFrameworkCore;
using PhoneDirectory.ContactService.Data;
using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ContactDbContext _contactDbContext;
        public PersonRepository(ContactDbContext contactDbContext)
        {
            _contactDbContext = contactDbContext;
        }
        public async Task AddAsync(Person person)
        {
            await _contactDbContext.Persons.AddAsync(person);
        }

        public void Delete(Person person)
        {
            _contactDbContext.Persons.Remove(person);
        }


        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            var persons = await _contactDbContext.Persons
                .Include(p => p.ContactInformations)
                .ToListAsync();
            return persons;
        }


        public async Task<Person?> GetByIdAsync(Guid id)
        {
            var person = await _contactDbContext.Persons
                 .Include(p => p.ContactInformations)
                 .FirstOrDefaultAsync(p => p.Id == id);
            return person;
        }

        public async Task SaveChangesAsync()
        {
            await _contactDbContext.SaveChangesAsync();
        }
    }
}
