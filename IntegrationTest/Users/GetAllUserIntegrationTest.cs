using Application.Results;
using Application.Users.Commands;
using Application.Users.Handlers;
using Application.Users.Queries;
using Docker.DotNet.Models;
using Domain.IRepository;
using IntegrationTest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTest.Users
{
    public class GetAllUserIntegrationTest : BaseIntegrationTest
    {
        public GetAllUserIntegrationTest(IntegrationTestWebApiFactory factory) : base(factory)
        {
        }
        [Fact]
        public async Task GetAllUserProfiles_Should_Return_AllUsersWithProfiles()
        {

            var command = new GetAllUserProfilesQuery();
;
            var totalNumber = seeder.GetUsers().Count;
            var result = await sender.Send(command); 

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value);
            Assert.Equal(result.Value.Count, totalNumber);


        }
    }
}
