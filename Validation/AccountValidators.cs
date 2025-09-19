using FluentValidation;

namespace Bank.Api.Validation;

public class AccountCreateValidator : AbstractValidator<AccountCreateDto>
{
    public AccountCreateValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.AccountType).NotEmpty();
        RuleFor(x => x.InitialBalance).GreaterThanOrEqualTo(0);
    }
}
