using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Services.PersonServices
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person?> GetByIdAsync(Guid id);
        Task AddAsync(Person person);
        Task<bool> DeleteAsync(Guid id);
        Task<(int personCount, int phoneCount)> GetStatsByLocationAsync(string location);
    }
}
