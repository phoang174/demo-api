using Domain.Entity;
namespace Application.IService

{
    public interface IJwtService
    {
        public string GenerateAccessToken(User user);
        public string GenerateRefreshToken(User user);


    }
}
