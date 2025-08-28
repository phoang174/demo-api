using Application.Users.Commands;
using Domain.Entity;
using IntegrationTest.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Users.Handlers.LoginCommandHandler;

namespace IntegrationTest.Users
{

    public class LoginIntegrationTest : BaseIntegrationTest
    {
        public LoginIntegrationTest(IntegrationTestWebApiFactory factory) : base(factory)
        {
        }
        [Fact]
        public async Task Login_ValidUsernameAndPassword_ShouldSucceed()
        {
            // Arrange
            var plainPassword = "Password123";
            var seededUser = seeder.GetUsers().FirstOrDefault();
            Assert.NotNull(seededUser);
      
            LoginCommand command = new LoginCommand{
                password = plainPassword,
                username = seededUser!.Username
            };
            var result = await sender.Send(command);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value!.AccessToken);
            Assert.NotNull(result.Value.RefreshToken);
            Assert.Equal(seededUser.Username, result.Value.User.Username);
            Assert.NotEmpty(result.Value.User.Roles);

        }
        [Fact]
        public async Task Login_InvalidPassword_ShouldFail()
        {
            var plainPassword = "WrongPassword";
            var seededUser = seeder.GetUsers().FirstOrDefault();
            Assert.NotNull(seededUser);

            LoginCommand command = new LoginCommand
            {
                password = plainPassword,
                username = seededUser!.Username
            };
            var result = await sender.Send(command);
            Assert.True(result.IsFailure);
            Assert.Equal(result.Error, LoginError.WrongUsernamOrPassword);
        }
    }
}
