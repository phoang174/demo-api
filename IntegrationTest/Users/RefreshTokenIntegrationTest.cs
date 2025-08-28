using Application.Users.Commands;
using IntegrationTest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Users.Handlers.RefreshTokenCommandHandler;

namespace IntegrationTest.Users
{

    public class RefreshTokenIntegrationTest : BaseIntegrationTest
    {
        public RefreshTokenIntegrationTest(IntegrationTestWebApiFactory factory) : base(factory)
        {
            
        }
        [Fact]
        public async Task RefreshToken_ValidToken_ShouldSucceded()
        {
            var seededUser = seeder.GetUsers().FirstOrDefault();
            var command = new RefreshTokenCommand
            {
                refreshToken = "validToken"
            };
            var result = await sender.Send(command);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value!.AccessToken);
            Assert.NotNull(result.Value.RefreshToken);
            Assert.Equal(seededUser.Username, result.Value.User.Username);
            Assert.NotEmpty(result.Value.User.Roles);
        }
        [Fact]
        public async Task RefreshToken_InValidToken_ShouldFail()
        {
            var seededUser = seeder.GetUsers().FirstOrDefault();
            var command = new RefreshTokenCommand
            {
                refreshToken = "invalidToken"
            };
            var result = await sender.Send(command);
            Assert.True(result.IsFailure);
 
            Assert.Equal(RefreshTokenCommandError.UserNotFound, result.Error);
        }
    }
}
