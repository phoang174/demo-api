using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public  interface IBlackListRepository : IRepository<BlackList>

    {
        Task<bool> CheckTokenExist(string accessToken);

    }
}
