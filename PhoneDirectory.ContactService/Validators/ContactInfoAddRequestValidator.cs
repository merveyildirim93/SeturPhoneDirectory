using FluentValidation;
using PhoneDirectory.ContactService.Dtos;

namespace PhoneDirectory.ContactService.Validators
{
    public class ContactInfoAddRequestValidator : AbstractValidator<ContactInfoRequestDto>
    {
        public ContactInfoAddRequestValidator()
        {
            RuleFor(x => x.Type)
                .Must(t => t == 1 || t == 2 || t == 3)
                .WithMessage("Geçersiz iletişim tipi. (1=Phone, 2=Email, 3=Location)");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("İletişim bilgisi boş olamaz.");

            When(x => x.Type == 1, () => { // Phone
                RuleFor(x => x.Value)
                    .Matches(@"^\+?[0-9\s\-()]{7,}$")
                    .WithMessage("Geçerli bir telefon numarası girin.");
            });

            When(x => x.Type == 2, () => { // Email
                RuleFor(x => x.Value)
                    .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");
            });

            When(x => x.Type == 3, () => { // Location
                RuleFor(x => x.Value)
                    .MinimumLength(2).WithMessage("Konum en az 2 karakter olmalı.");
            });
        }
    }
}
