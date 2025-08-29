using demo_api;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.MsSql;

namespace IntegrationTest.Infrastructure
{
    public class IntegrationTestWebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer;
        public IntegrationTestWebApiFactory()
        {
            _dbContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Your_password123")
                .WithCleanUp(true)
                .Build();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services => {
            var descriptor = services.SingleOrDefault(s=>s.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if(descriptor is not null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<AppDbContext>(options =>
                {
                    options
                    .UseSqlServer(_dbContainer.GetConnectionString());
                    //.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
                });
         
            });
        }
        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }
    }
}
