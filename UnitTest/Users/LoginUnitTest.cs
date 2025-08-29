using Application.Dtos;
using Application.Results;
using Application.Users.Commands;
using Application.Users.Handlers;
using Domain.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnitTest.Infrastruture.UnitTest;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Application.IService;

namespace UnitTest.Users
{
    public class LoginUnitTest : BaseUnitTest
    {
        private readonly Mock<IJwtService> _jwtService;

        public LoginUnitTest()
        {
            _jwtService = new Mock<IJwtService>();
        }

        [Fact]
        public async Task Handle_Should_Login_Success()
        {
            // Arrange
            var password = "123456";
            var command = new LoginCommand
            {
                username = "test_user",
                password = password
            };

            var fakeUser = new User
            {
                Id = 1,
                Username = "test_user",
                Password = new PasswordHasher<User>().HashPassword(null!, password)
            };

            _userRepository.Setup(r => r.GetByUsernameAsync(command.username))
                           .ReturnsAsync(fakeUser);

            _jwtService.Setup(s => s.GenerateRefreshToken(fakeUser))
                       .Returns("fake_refresh_token");
            _jwtService.Setup(s => s.GenerateAccessToken(fakeUser))
                       .Returns("fake_access_token");

            _userRepository.Setup(r => r.UpdateAsync(fakeUser))
                           .Returns(Task.CompletedTask);

            _userRepository.Setup(r => r.GetUserRolesAsync(fakeUser.Id))
                           .ReturnsAsync(new List<string> { "Admin", "User" });

            var handler = new LoginCommandHandler(_userRepository.Object, _jwtService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("fake_access_token", result.Value.AccessToken);
            Assert.Equal("fake_refresh_token", result.Value.RefreshToken);
            Assert.Equal("test_user", result.Value.User.Username);
            Assert.Contains("Admin", result.Value.User.Roles);
            Assert.Contains("User", result.Value.User.Roles);

            _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.RefreshToken == "fake_refresh_token")), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnError_When_InvalidCredentials()
        {
            // Arrange
            var command = new LoginCommand
            {
                username = "wrong_user",
                password = "wrong_password"
            };

            _userRepository.Setup(r => r.GetByUsernameAsync(command.username))
                           .ReturnsAsync((User)null);

            var handler = new LoginCommandHandler(_userRepository.Object, _jwtService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(LoginCommandHandler.LoginError.WrongUsernamOrPassword, result.Error);

            _jwtService.Verify(s => s.GenerateAccessToken(It.IsAny<User>()), Times.Never);
            _jwtService.Verify(s => s.GenerateRefreshToken(It.IsAny<User>()), Times.Never);
            _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
