using Application.Dtos;
using Application.Results;
using Domain.Entity;
using MediatR;


namespace Application.Users.Commands
{
    public class CreateUserCommand : IRequest<Result<UserProfile>>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public List<int> Roles { get; set; } = [];
    }

}
