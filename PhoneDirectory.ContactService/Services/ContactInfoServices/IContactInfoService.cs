using PhoneDirectory.Core.Entities;

namespace PhoneDirectory.ContactService.Services.ContactInfoServices
{
    public interface IContactInfoService
    {
        Task<ContactInformation?> AddContactInfoAsync(Guid personId, ContactInformation info);
        Task<bool> RemoveContactInfoAsync(Guid personId, Guid contactInfoId);
    }
}
