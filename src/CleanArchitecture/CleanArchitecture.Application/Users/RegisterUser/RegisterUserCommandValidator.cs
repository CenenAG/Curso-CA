using FluentValidation;

namespace CleanArchitecture.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(5)
            .MaximumLength(100)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("Nombre is required")
            .MaximumLength(200);

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .WithMessage("Apellido is required")
            .MaximumLength(200);
    }
}
