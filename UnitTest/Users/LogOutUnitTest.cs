using Application.Users.Commands;
using Application.Users.Handlers;
using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Repositories;
using Moq;
using UnitTest.Infrastruture.UnitTest;
namespace UnitTest.Users
{
    public class LogOutUnitTest : BaseUnitTest
    {
      

        [Fact]
        public async Task Hanlde_Should_LogOut_Sucess()
        {
            var command = new LogoutCommand
            {
                accessToken = "AccessToken",
                userId = 1
            };
            var fakeUser = new User
            {
                Id = 1,
                RefreshToken = "OldToken"
            };

            _userRepository
               .Setup(repo => repo.GetByIdAsync(command.userId))
               .ReturnsAsync(fakeUser);
            _blackListRepository
                .Setup(repo => repo.AddAsync(It.IsAny<BlackList>()))
                .ReturnsAsync(new BlackList
                {
                    AccessToken = command.accessToken,
                    RevokedAt = DateTime.UtcNow
                });

            _userRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            var handler = new LogoutCommandHandler(_userRepository.Object, _blackListRepository.Object);


            var result = await handler.Handle(command, CancellationToken.None);



            Assert.True(result.IsSuccess);
            _blackListRepository.Verify(repo => repo.AddAsync(It.Is<BlackList>(
                b => b.AccessToken == "AccessToken"
            )), Times.Once);
            _userRepository.Verify(repo => repo.UpdateAsync(It.Is<User>(
                u => u.RefreshToken == string.Empty
            )), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_LogOut_NotFound()
        {
            var command = new LogoutCommand
            {
                accessToken = "AccessToken",
                userId = 1
            };

            _userRepository
               .Setup(repo => repo.GetByIdAsync(command.userId))
               .ReturnsAsync((User?)null);

            var handler = new LogoutCommandHandler(_userRepository.Object, _blackListRepository.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal("LogoutCommandHandler.Notfound", result.Error?.Message);
            Assert.Equal("User not found", result.Error?.Detail);
            Assert.Equal(400, result.Error?.Code);

            _blackListRepository.Verify(repo => repo.AddAsync(It.IsAny<BlackList>()), Times.Never);
            _userRepository.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

    }
}
