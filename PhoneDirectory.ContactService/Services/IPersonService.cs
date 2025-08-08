using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person?> GetByIdAsync(Guid id);
        Task AddAsync(Person person);
        Task<bool> DeleteAsync(Guid id);
    }
}
