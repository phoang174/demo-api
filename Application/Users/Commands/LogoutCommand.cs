using Application.Results;
using Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class LogoutCommand: IRequest<Result>
    {
        public int userId { get; set; }
        public string accessToken { get; set; }
    
    }
}
