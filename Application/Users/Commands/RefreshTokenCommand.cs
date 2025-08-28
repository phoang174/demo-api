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
    public class RefreshTokenCommand : IRequest<Result<LoginResult>>
    {
        public string refreshToken { get; set; }
    }
}
