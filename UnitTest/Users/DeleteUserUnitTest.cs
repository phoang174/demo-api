using Application.Users.Commands;
using Application.Users.Handlers;
using Domain.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Infrastruture.UnitTest;

namespace UnitTest.Users
{
    public class DeleteUserUnitTest : BaseUnitTest
    {

        [Fact]
        public async Task Handle_Should_DeleteUser_Success()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Password = "123456",
          
            };

            _userRepository.Setup(r => r.GetByIdAsync(userId))
                               .ReturnsAsync(user);

            _userRepository.Setup(r => r.DeleteAsync(userId))
                               .Returns(Task.CompletedTask);

            var handler = new DeleteUserCommandHandler(_userRepository.Object);
            var command = new DeleteUserCommand
            {
                userId = userId,
            };
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_When_UserNotFound()
        {
            var userId = 100;

            _userRepository.Setup(r => r.GetByIdAsync(userId))
                               .ReturnsAsync((User)null);

            var handler = new DeleteUserCommandHandler(_userRepository.Object);
            var command = new DeleteUserCommand
            {
                userId=userId,
            };
            var result = await handler.Handle(command, CancellationToken.None);

          

            Assert.False(result.IsSuccess);
            Assert.Equal(DeleteUserError.UserNotFound, result.Error);
        }
    }
}
