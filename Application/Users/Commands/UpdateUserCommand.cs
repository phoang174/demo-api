using Application.Dtos;
using Application.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result<UserProfile>>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public List<int> Roles { get; set; } = [];
    }
}
