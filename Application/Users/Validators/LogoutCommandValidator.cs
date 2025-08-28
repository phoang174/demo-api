using Application.Users.Commands;
using FluentValidation;

namespace Application.Users.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.userId)
                .GreaterThan(0).WithMessage("User Id must be greater than 0");

            RuleFor(x => x.accessToken)
                .NotEmpty().WithMessage("Access token is required");
        }
    }
}
