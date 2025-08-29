using Application.Users.Commands;
using FluentValidation;

namespace Application.Users.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.refreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        }
    }
}
