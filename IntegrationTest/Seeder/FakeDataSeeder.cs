using Bogus;
using Domain.Entity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationTest.Seeder
{
    public class FakeDataSeeder
    {
        private readonly AppDbContext _context;
        private readonly Faker _faker;

        public FakeDataSeeder(AppDbContext context)
        {
            _context = context;
            _faker = new Faker();
        }

        public void SeedAll(int numberOfUsers = 10)
        {
            SeedRoles();
            var users = SeedUsers(numberOfUsers);
            _context.SaveChanges(); 

            SeedProfiles(users);
            SeedUserRoles(users);
            _context.SaveChanges();
        }

        private void SeedRoles()
        {
            if (!_context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Staff" }
                };
                _context.Roles.AddRange(roles);
            }
        }

        private List<User> SeedUsers(int count)
        {
            Console.WriteLine("Seeding users...");
            var hasher = new PasswordHasher<User>();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Password, (f, u) => hasher.HashPassword(u, "Password123"))
                .RuleFor(u => u.RefreshToken, f => "validToken");


            var users = userFaker.Generate(count);
            _context.Users.AddRange(users);
            return users;
        }

        private void SeedProfiles(List<User> users)
        {
            var profileFaker = new Faker<Profile>()
                .RuleFor(p => p.UserId, f => users[f.IndexFaker % users.Count].Id)
                .RuleFor(p => p.Email, f => _faker.Internet.Email());

            var profiles = profileFaker.Generate(users.Count);
            _context.Profiles.AddRange(profiles);
        }

        private void SeedUserRoles(List<User> users)
        {
            var roles = _context.Roles.ToList();
            var userRoles = new List<UserRole>();
            foreach (var user in users)
            {
                var roleId = roles[_faker.Random.Int(0, roles.Count - 1)].Id;
                userRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }
            _context.UserRoles.AddRange(userRoles);
        }


        public List<User> GetUsers() => _context.Users.ToList();

        public User? GetUserById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);

        public List<Role> GetRoles() => _context.Roles.ToList();

        public List<UserRole> GetUserRoles() => _context.UserRoles.ToList();

        public List<Profile> GetProfiles() => _context.Profiles.ToList();
    }
}
