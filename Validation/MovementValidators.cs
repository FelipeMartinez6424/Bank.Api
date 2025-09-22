using FluentValidation;
using Bank.Api.Validation;

public class MovementCreateDtoValidator : AbstractValidator<MovementCreateDto>
{
    public MovementCreateDtoValidator()
    {
        RuleFor(x => x.AccountId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.MovementType).IsInEnum();
    }
}

