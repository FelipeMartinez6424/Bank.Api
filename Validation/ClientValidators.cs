using FluentValidation;

namespace Bank.Api.Validation;

public class ClientCreateValidator : AbstractValidator<ClientCreateDto>
{
    public ClientCreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).NotEmpty();
        RuleFor(x => x.Age).InclusiveBetween(1, 120);
        RuleFor(x => x.Identification).NotEmpty();
        RuleFor(x => x.ClientCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(3);
    }
}

