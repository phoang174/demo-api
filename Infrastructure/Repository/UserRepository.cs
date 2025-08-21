using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _dbSet.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
            await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        public async Task<List<string>> GetUserRolesAsync(int userId) =>
            await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UserRole)
                .ThenInclude(ur => ur.Role)
                .SelectMany(u => u.UserRole.Select(ur => ur.Role.RoleName))
                .ToListAsync();

        public override async Task<List<User>> GetAllItemAsync()
        {
            return await _dbSet
                .Include(u => u.Profile) 
                .ToListAsync();
        }
        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Profile)
                .Include(e=>e.UserRole)
                .ThenInclude(e=>e.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }


    }
}
