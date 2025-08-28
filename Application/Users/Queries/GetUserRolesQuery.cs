using Application.Results;
using Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public class GetUserRolesQuery :IRequest<Result<List<Role>>>
    {
    }
}
