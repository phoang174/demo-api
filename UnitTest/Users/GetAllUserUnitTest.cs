using Application.Dtos;
using Application.Results;
using Application.Users.Handlers;
using Application.Users.Queries;
using Domain.Entity;
using Domain.IRepository;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnitTest.Infrastruture.UnitTest;
using Xunit;

namespace UnitTest.Users
{
    public class GetAllUserProfilesQueryHandlerTests : BaseUnitTest
    {
        private readonly GetAllUserProfilesQueryHandler _handler;



        [Fact]
        public async Task Handle_ShouldReturnUserProfiles_WhenUsersExist()
        {
            GetAllUserProfilesQueryHandler _handler = new GetAllUserProfilesQueryHandler(_userRepository.Object);

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "john_doe",
                    Profile = new Profile
                    {
                        Birthday = new  DateTime(2020, 12, 12),
                        Email = "john@example.com"
                    }
                },
                new User
                {
                    Id = 2,
                    Username = "jane_doe",
                    Profile = new Profile
                    {
                        Birthday = new  DateTime(2020, 12, 12),
                        Email = "jane@example.com"
                    }
                }
            };

            _userRepository.Setup(repo => repo.GetAllItemAsync())
                .ReturnsAsync(users);

            _userRepository.Setup(repo => repo.GetUserRolesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<string> { "Admin", "User" });

            var query = new GetAllUserProfilesQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.Contains(result.Value, u => u.Username == "john_doe" && u.Roles.Contains("Admin"));
        }

       
    }
}
