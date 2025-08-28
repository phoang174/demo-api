using Application.Users.Commands;
using Domain.Entity;
using IntegrationTest.Infrastructure;
using IntegrationTest.Seeder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTest.Users
{

    public class LogoutIntegrationTest : BaseIntegrationTest
    {
        public LogoutIntegrationTest(IntegrationTestWebApiFactory factory) : base(factory)
        {
        }
        [Fact]
        public async Task Logout_Should_Succeed_When_User_Exists()
        {
            // Arrange
           
            var seededUser = seeder.GetUsers().FirstOrDefault();
            Assert.NotNull(seededUser); 

            seededUser!.RefreshToken = "OldRefreshToken";
            await dbContext.SaveChangesAsync();

            var command = new LogoutCommand
            {
                userId = seededUser.Id,
                accessToken = "AccessToken123"
            };

            // Act
            var result = await sender.Send(command);

            // Assert
            Assert.True(result.IsSuccess);

            
            var updatedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == seededUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(string.Empty, updatedUser!.RefreshToken);

            var blackListEntry = await dbContext.BlackLists.FirstOrDefaultAsync(b => b.AccessToken == "AccessToken123");
            Assert.NotNull(blackListEntry);
        }
    }
}
