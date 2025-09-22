using FluentValidation;

namespace Bank.Api.Validation;

public class ClientCreateValidator : AbstractValidator<ClientCreateDto>
{
    public ClientCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(80).WithMessage("El nombre no debe superar 80 caracteres.")
            .Matches(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]+$")
            .WithMessage("El nombre solo debe contener letras y espacios.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("El género es obligatorio.")
            .Must(g => g == "M" || g == "F")
            .WithMessage("El género debe ser 'M' o 'F'.");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120)
            .WithMessage("La edad debe estar entre 18 y 120.");

        RuleFor(x => x.Identification)
            .NotEmpty().WithMessage("La identificación es obligatoria.")
            .MaximumLength(30);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("La dirección es obligatoria.")
            .MaximumLength(120);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El celular es obligatorio.")
            .Matches(@"^[0-9]{7,15}$")
            .WithMessage("El celular debe tener entre 7 y 15 dígitos.");

        RuleFor(x => x.ClientCode)
            .NotEmpty().WithMessage("El código de cliente es obligatorio.")
            .Matches(@"^[A-Za-z0-9]{4,20}$")
            .WithMessage("El código de cliente debe ser alfanumérico (4-20).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(4).WithMessage("La contraseña debe tener al menos 4 caracteres.");
    }
}

