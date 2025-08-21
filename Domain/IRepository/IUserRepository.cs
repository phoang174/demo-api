using Domain.Entity;


namespace Domain.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
      
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<List<string>> GetUserRolesAsync(int userId);
    }
}
