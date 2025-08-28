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
    public class CreateUserUnitTest : BaseUnitTest
    {
       
            [Fact]
            public async Task Handle_Should_CreateUser_Success()
            {
                // Arrange
                var command = new CreateUserCommand
                {
                    Username = "new_user",
                    Email = "newuser@test.com",
                    Birthday = new DateTime(2020, 12, 12),
                    Roles = new List<int> { 1, 2 }
                };

                var fakeUser = new User
                {
                    Id = 1,
                    Username = "new_user",
                    Profile = new Profile
                    {
                        Email = "newuser@test.com",
                        Birthday = new DateTime(2020, 12, 12), 
                    }
                };

                _userRepository
                .Setup(repo => repo.GetByUsernameAsync(command.Username))
                .ReturnsAsync((User?)null); 

                _userRepository
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(fakeUser); 

               

                _userRepository
                .Setup(repo => repo.GetUserRolesAsync(fakeUser.Id))
                .ReturnsAsync(new List<string> { "Admin", "User" });


            var handler = new CreateUserCommandHandler(_userRepository.Object);

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

            // Assert
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);
                Assert.Equal("new_user", result.Value.Username);
                Assert.Equal("newuser@test.com", result.Value.Email);
                Assert.Equal(new  DateTime(2020, 12, 12), result.Value.Birthday);
                Assert.Contains("Admin", result.Value.Roles);
                Assert.Contains("User", result.Value.Roles);

                _userRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
                _userRepository.Verify(repo => repo.GetUserRolesAsync(fakeUser.Id), Times.Once);
            }

            [Fact]
            public async Task Handle_Should_Fail_When_UsernameExists()
            {
                // Arrange
                var command = new CreateUserCommand
                {
                    Username = "existing_user",
                    Email = "existing@test.com",
                    Birthday = new DateTime(1990, 1, 1),
                    Roles = new List<int> { 1 }
                };

                var existingUser = new User
                {
                    Id = 99,
                    Username = "existing_user",
                    Profile = new Profile
                    {
                        Email = "existing@test.com",
                        Birthday = new DateTime(1990, 1, 1)
                    }
                };

                _userRepository
                    .Setup(repo => repo.GetByUsernameAsync(command.Username))
                    .ReturnsAsync(existingUser);

                var handler = new CreateUserCommandHandler(_userRepository.Object);

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Equal(CreateUserError.UsernameAlreadyExists, result.Error);

            }
       
    }
}
