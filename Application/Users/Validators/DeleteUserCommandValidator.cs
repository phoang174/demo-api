using Application.Users.Commands;
using FluentValidation;

namespace Application.Users.Validators
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.userId)
                .GreaterThan(0).WithMessage("User Id must be greater than 0");
        }
    }
}
