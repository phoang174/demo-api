using Application.Users.Queries;
using Domain.Entity;
using Domain.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Handlers
{
    public class GetUserRoleQueryHandler : IRequestHandler<GetUserRolesQuery, List<Role>>
    {
        private readonly IRoleRepository roleRepository;
        public GetUserRoleQueryHandler(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }
        public async Task<List<Role>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            return await this.roleRepository.GetAllItemAsync();
        }
    }
}
