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
    public class LoginCommand : IRequest<Result<LoginResult>>
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
