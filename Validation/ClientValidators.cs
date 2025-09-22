using FluentValidation;

namespace Bank.Api.Validation;

public class ClientUpdateValidator : AbstractValidator<ClientUpdateDto>
{
    public ClientUpdateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 120);
        RuleFor(x => x.Identification).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
    }
}


