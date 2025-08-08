using FluentValidation;
using PhoneDirectory.ContactService.Dtos;

namespace PhoneDirectory.ContactService.Validators
{
    public class PersonCreateRequestValidator : AbstractValidator<PersonCreateRequestDto>
    {
        public PersonCreateRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad zorunludur.")
                .MinimumLength(3).WithMessage("Ad en az 3 karakter olmalı.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad zorunludur.")
                .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalı.");

            RuleFor(x => x.Company)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Company));

            RuleForEach(x => x.Contacts)
                .SetValidator(new ContactInfoRequestValidator());
        }
    }
}
