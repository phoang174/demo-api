using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IRoleService
    {
        public Task<List<Role>> getAllRoles();
    }
}
