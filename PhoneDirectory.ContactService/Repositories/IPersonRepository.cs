using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Repositories
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person?> GetByIdAsync(Guid id);
        Task AddAsync(Person person);
        void Delete(Person person);
        Task SaveChangesAsync();
    }
}
