using FluentAssertions;
using PhoneDirectory.ContactService.Repositories;
using PhoneDirectory.ContactService.Services.ContactInfoServices;
using PhoneDirectory.ContactService.Tests.TestHelpers;
using PhoneDirectory.Core.Entities;
using PhoneDirectory.Core.Enums;
using Xunit;

namespace PhoneDirectory.ContactService.Tests.Services
{
    public class ContactInfoServiceTests
    {
        [Fact]
        public async Task AddContactInfoAsync_ShouldAddInfo_WhenPersonExists()
        {
            var (ctx, conn) = SqliteInMemory.CreateContactContext();
            try
            {
                var repo = new PersonRepository(ctx);
                var service = new ContactInfoService(repo, ctx);

                var person = new Person
                {
                    FirstName = "Merve",
                    LastName = "Yıldırım",
                    Company = "TestCo"
                };
                await ctx.Persons.AddAsync(person);
                await ctx.SaveChangesAsync();

                var info = new ContactInformation
                {
                    Type = ContactType.Email,
                    Value = "merve@example.com"
                };

                var added = await service.AddContactInfoAsync(person.Id, info);

                added.Should().NotBeNull();
                var saved = await repo.GetByIdAsync(person.Id);
                saved!.ContactInformations.Should().ContainSingle(ci =>
                    ci.Type == ContactType.Email && ci.Value == "merve@example.com");
            }
            finally { conn.Dispose(); }
        }

        [Fact]
        public async Task RemoveContactInfoAsync_ShouldRemoveInfo_WhenExists()
        {
            var (ctx, conn) = SqliteInMemory.CreateContactContext();
            try
            {
                var repo = new PersonRepository(ctx);
                var service = new ContactInfoService(repo, ctx);

                var person = new Person { FirstName = "Ali", LastName = "Veli", Company = "TestCo" };
                var ci = new ContactInformation { Type = ContactType.Phone, Value = "+90 555 123 4567" };
                person.ContactInformations.Add(ci);

                await ctx.Persons.AddAsync(person);
                await ctx.SaveChangesAsync();

                var ok = await service.RemoveContactInfoAsync(person.Id, ci.Id);
                ok.Should().BeTrue();

                var saved = await repo.GetByIdAsync(person.Id);
                saved!.ContactInformations.Should().BeEmpty();
            }
            finally { conn.Dispose(); }
        }
    }
}
