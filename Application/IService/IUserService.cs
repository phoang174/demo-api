using Application.Dtos;
using Domain.Entity;
namespace Application.IService
{
    public interface IUserService
    {
        public User createUser(string username, string password);
        public Task<LoginResult> Login(string username, string password);
        public Task<LoginResult> RefreshTokenHandler (string refreshToken);
        public Task<List<string>> getUserRole(int userId);
        public Task<bool> LogOutAsync(int userId, string accessToken);
        public Task<List<UserProfile>> GetAllUserProfiles();
        public Task<UserProfile> CreateUserProfile(CreateProfileDto createProfileDto);
        public Task<UserProfile> UpdateUser(UpdateUserDto updateUserDto);
        public Task DeleteUser(int userId);
    }
}
