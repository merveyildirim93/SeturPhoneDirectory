using PhoneDirectory.ContactService.Repositories;
using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Services.PersonServices
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _personRepository.GetAllAsync();
        }

        public async Task<Person?> GetByIdAsync(Guid id)
        {
            return await _personRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Person person)
        {
            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
                return false;

            _personRepository.Delete(person);
            await _personRepository.SaveChangesAsync();
            return true;
        }

        public async  Task<(int personCount, int phoneCount)> GetStatsByLocationAsync(string location)
        {
            return await _personRepository.GetStatsByLocationAsync(location);
        }

        public async Task<bool> UpdateAsync(Person person)
        {
            if (person == null)
                return false;

            await _personRepository.UpdateAsync(person);
            await _personRepository.SaveChangesAsync();
            return true;
        }
    }
}
