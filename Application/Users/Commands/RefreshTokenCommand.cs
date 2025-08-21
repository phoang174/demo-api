using Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResult>
    {
        public string refreshToken { get; set; }
    }
}
