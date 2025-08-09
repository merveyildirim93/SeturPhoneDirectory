using FluentAssertions;
using PhoneDirectory.ContactService.Repositories;
using PhoneDirectory.ContactService.Services;
using PhoneDirectory.ContactService.Services.ContactInfoServices;
using PhoneDirectory.ContactService.Services.PersonServices;
using PhoneDirectory.ContactService.Tests.TestHelpers;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;

namespace PhoneDirectory.ContactService.Tests.Services
{
    public class PersonServiceTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddPersonWithContacts()
        {
            var (ctx, conn) = SqliteInMemory.CreateContactContext();
            try
            {
                var repo = new PersonRepository(ctx);
                var service = new PersonService(repo);

                var person = new Person
                {
                    FirstName = "Merve",
                    LastName = "Yıldırım",
                    Company = "Test Ltd",
                    ContactInformations = new List<ContactInformation>
                    {
                        new() { Type = ContactType.Location, Value = "Kadıköy" },
                        new() { Type = ContactType.Phone, Value = "+90 555 111 2233" }
                    }
                };

                await service.AddAsync(person);

                var saved = await service.GetByIdAsync(person.Id);
                saved.Should().NotBeNull();
                saved!.ContactInformations.Should().HaveCount(2);
            }
            finally { conn.Dispose(); }
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            var (ctx, conn) = SqliteInMemory.CreateContactContext();
            try
            {
                var repo = new PersonRepository(ctx);
                var service = new PersonService(repo);

                var result = await service.DeleteAsync(Guid.NewGuid());
                result.Should().BeFalse();
            }
            finally { conn.Dispose(); }
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenFound()
        {
            var (ctx, conn) = SqliteInMemory.CreateContactContext();
            try
            {
                var repo = new PersonRepository(ctx);
                var service = new PersonService(repo);

                var person = new Person { FirstName = "Onur", LastName = "Yıldırım", Company = "TestCo" };
                await service.AddAsync(person);
                var result = await service.DeleteAsync(person.Id);

                result.Should().BeTrue();
            }
            finally { conn.Dispose(); }
        }

    }
}
