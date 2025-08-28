using Application.Dtos;
using Application.Results;
using Application.Users.Commands;
using Application.Users.Handlers;
using IntegrationTest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest.Users
{

    public class UpdateUserIntegrationTest : BaseIntegrationTest
    {
        public UpdateUserIntegrationTest(IntegrationTestWebApiFactory factory)
            : base(factory) { }

        [Fact]
        public async Task UpdateUser_Should_Succeed()
        {
            var seededUser = seeder.GetUsers().FirstOrDefault();
            Assert.NotNull(seededUser);

            var newRoles = seeder.GetRoles().Select(r => r.Id).ToList(); 
            var command = new UpdateUserCommand
            {
                Id = seededUser!.Id,
                Username = "UpdatedUsername",
                Email = "updated@example.com",
                Birthday = new DateTime(1995, 5, 5),
                Roles = newRoles
            };

            var result = await sender.Send(command);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Username, result.Value.Username);
            Assert.Equal(command.Email, result.Value.Email);
            Assert.Equal(command.Birthday, result.Value.Birthday);
            Assert.Equal(newRoles.Count, result.Value.Roles.Count);
        }

        [Fact]
        public async Task UpdateUser_NonExistingUser_ShouldFail()
        {
            var command = new UpdateUserCommand
            {
                Id = 99999, 
                Username = "DoesNotExist",
                Email = "no@example.com",
                Birthday = new DateTime(2000, 1, 1),
                Roles = new List<int> { 1 }
            };

            // Act
            var result = await sender.Send(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(UpdateUserError.UserNotFound, result.Error);
        }
    }
}
