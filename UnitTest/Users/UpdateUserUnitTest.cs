using Application.Dtos;
using Application.Results;
using Application.Users.Commands;
using Application.Users.Handlers;
using Domain.Entity;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitTest.Infrastruture.UnitTest;
using Xunit;

namespace UnitTest.Users
{
    public class UpdateUserUnitTest : BaseUnitTest
    {
        [Fact]
        public async Task Handle_Should_UpdateUser_Success()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                Id = 1,
                Username = "updated_user",
                Birthday = new DateTime(2020, 12, 12),
                Email = "updated@example.com",
                Roles = new List<int> { 1, 2 }
            };

            var fakeUser = new User
            {
                Id = 1,
                Username = "old_user",
                Profile = new Profile
                {
                    Birthday = new DateTime(2020, 12, 12),
                    Email = "old@example.com"
                },
                UserRole = new List<UserRole>
                {
                    new UserRole { UserId = 1, RoleId = 1, Role = new Role { Id = 1, RoleName = "Admin" } }
                }
            };

            var updatedUser = new User
            {
                Id = 1,
                Username = "updated_user",
                Profile = new Profile
                {
                    Birthday = new DateTime(2020, 12, 12),
                    Email = "updated@example.com"
                },
                UserRole = new List<UserRole>
                {
                    new UserRole { UserId = 1, RoleId = 1, Role = new Role { Id = 1, RoleName = "Admin" } },
                    new UserRole { UserId = 1, RoleId = 2, Role = new Role { Id = 2, RoleName = "User" } }
                }
            };

            _userRepository.Setup(r => r.GetByIdAsync(command.Id))
                           .ReturnsAsync(fakeUser);

            _userRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                           .Returns(Task.CompletedTask);

            _userRepository.Setup(r => r.GetByIdAsync(fakeUser.Id))
                           .ReturnsAsync(updatedUser);

            var handler = new UpdateUserCommandHandler(_userRepository.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Username, result.Value.Username);
            Assert.Equal(command.Birthday, result.Value.Birthday);
            Assert.Equal(command.Email, result.Value.Email);
            Assert.Contains("Admin", result.Value.Roles);
            Assert.Contains("User", result.Value.Roles);

            _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.Username == command.Username &&
                u.Profile.Email == command.Email)), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_When_UserNotFound()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                Id = 999,
                Username = "not_exist_user",
                Birthday = new DateTime(2020, 12, 12),
                Email = "notfound@example.com",
                Roles = new List<int> { 1 }
            };

            _userRepository.Setup(r => r.GetByIdAsync(command.Id))
                           .ReturnsAsync((User)null);

            var handler = new UpdateUserCommandHandler(_userRepository.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(UpdateUserError.UserNotFound, result.Error);

            _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
