using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class BlackListRepository : Repository<BlackList>, IBlackListRepository
    {
        public BlackListRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckTokenExist(string accessToken)
        {
            var res = await this._dbSet.FirstOrDefaultAsync(e=>e.AccessToken == accessToken);
            if (res != null) {
                return true;
            }
            return false;
        }
    }
}
