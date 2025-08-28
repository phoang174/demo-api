using Application.Dtos;
using Application.IService;
using Application.Results;
using Application.Users.Commands;
using Application.Users.Handlers;
using Domain.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnitTest.Infrastruture.UnitTest;
using Xunit;

namespace UnitTest.Users
{
    public class RefreshTokenUnitTest : BaseUnitTest
    {
        private readonly Mock<IJwtService> _jwtService;

        public RefreshTokenUnitTest()
        {
            _jwtService = new Mock<IJwtService>();
        }

        [Fact]
        public async Task Handle_Should_RefreshToken_Success()
        {
            // Arrange
            var oldToken = "old_refresh_token";
            var command = new RefreshTokenCommand
            {
                refreshToken = oldToken
            };

            var fakeUser = new User
            {
                Id = 1,
                Username = "test_user",
                RefreshToken = oldToken
            };

            _userRepository.Setup(r => r.GetByRefreshTokenAsync(command.refreshToken))
                           .ReturnsAsync(fakeUser);

            _jwtService.Setup(s => s.GenerateRefreshToken(fakeUser))
                       .Returns("new_refresh_token");
            _jwtService.Setup(s => s.GenerateAccessToken(fakeUser))
                       .Returns("new_access_token");

            _userRepository.Setup(r => r.UpdateAsync(fakeUser))
                           .Returns(Task.CompletedTask);

            _userRepository.Setup(r => r.GetUserRolesAsync(fakeUser.Id))
                           .ReturnsAsync(new List<string> { "Admin", "User" });

            var handler = new RefreshTokenCommandHandler(_userRepository.Object, _jwtService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("new_access_token", result.Value.AccessToken);
            Assert.Equal("new_refresh_token", result.Value.RefreshToken);
            Assert.Equal("test_user", result.Value.User.Username);
            Assert.Contains("Admin", result.Value.User.Roles);
            Assert.Contains("User", result.Value.User.Roles);

            _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.RefreshToken == "new_refresh_token")), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_When_UserNotFound()
        {
            // Arrange
            var command = new RefreshTokenCommand
            {
                refreshToken = "invalid_token"
            };

            _userRepository.Setup(r => r.GetByRefreshTokenAsync(command.refreshToken))
                           .ReturnsAsync((User)null);

            var handler = new RefreshTokenCommandHandler(_userRepository.Object, _jwtService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(RefreshTokenCommandHandler.RefreshTokenCommandError.UserNotFound, result.Error);

            _jwtService.Verify(s => s.GenerateAccessToken(It.IsAny<User>()), Times.Never);
            _jwtService.Verify(s => s.GenerateRefreshToken(It.IsAny<User>()), Times.Never);
            _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
