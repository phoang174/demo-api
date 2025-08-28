using Application.Users.Commands;
using FluentValidation;

namespace Application.Users.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be valid");

            RuleFor(x => x.Birthday)
                .LessThan(DateTime.Now).WithMessage("Birthday must be in the past");

            RuleFor(x => x.Roles)
                .NotNull().WithMessage("Roles cannot be null")
                .Must(r => r.Count > 0).WithMessage("At least one role must be assigned");
        }
    }
}
