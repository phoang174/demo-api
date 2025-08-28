using Infrastructure.Data;
using IntegrationTest.Seeder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace IntegrationTest.Infrastructure
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApiFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly ISender sender;
        protected readonly AppDbContext dbContext;
        protected readonly FakeDataSeeder seeder;

        protected BaseIntegrationTest(IntegrationTestWebApiFactory factory)
        {

            _scope = factory.Services.CreateScope();
            sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (dbContext.Database.GetPendingMigrations().Any()) {
                dbContext.Database.Migrate();
            }
            seeder = new FakeDataSeeder(dbContext);
            seeder.SeedAll(10);
        }
       
    }
}
