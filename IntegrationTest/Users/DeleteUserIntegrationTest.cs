using Application.Users.Commands;
using Application.Users.Handlers;
using Infrastructure.Repositories;
using IntegrationTest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTest.Users
{

    public class DeleteUserIntegrationTest : BaseIntegrationTest
    {
        public DeleteUserIntegrationTest(IntegrationTestWebApiFactory factory) : base(factory)
        {
        }
        [Fact]  
        public async Task DeleteUser_UserNotFound_ShouldFail()
        {
            // Arrange
            var command = new DeleteUserCommand
            {
                userId = 100
            };

            // Act
            var result = await sender.Send(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(DeleteUserError.UserNotFound, result.Error);
        }

        [Fact]
        public async Task DeleteUser_ValidUser_ShouldSucceed()
        {
            var seededUser = seeder.GetUsers().FirstOrDefault();
            Assert.NotNull(seededUser);

            var command = new DeleteUserCommand
            {
                userId = seededUser!.Id
            };

            // Act
            var result = await sender.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

           
        }
    }
}
