using Application.Users.Commands;
using Application.Users.Handlers;
using IntegrationTest.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTest.Users
{

    public class CreateUserIntegrationTest : BaseIntegrationTest
    {
        public CreateUserIntegrationTest(IntegrationTestWebApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ShouldSucceed()
        {
            var roles = dbContext.Roles.ToList();
            var adminRoleId = roles.First(r => r.RoleName == "Admin").Id;
            var staffRoleId = roles.First(r => r.RoleName == "Staff").Id;
            var command = new CreateUserCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Birthday = DateTime.Parse("2000-01-01"),
                Roles = new List<int> { adminRoleId, staffRoleId } 
            };

            var result = await sender.Send(command);
            
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Username, result.Value.Username);
            Assert.Equal(command.Email, result.Value.Email);
            Assert.Equal(2, result.Value.Roles.Count);
        }
        [Fact]
        public async Task CreateUser_ExistUsername_ShouldFail()
        {
            var user = seeder.GetUsers().FirstOrDefault();
            var command = new CreateUserCommand
            {
                Username = user.Username,
                Email = "newuser@example.com",
                Birthday = DateTime.Parse("2000-01-01"),
                Roles = new List<int> { 1, 2 }
            };

            var result = await sender.Send(command);

            Assert.True(result.IsFailure);

            Assert.Equal(CreateUserError.UsernameAlreadyExists, result.Error);
        }
    }
}
