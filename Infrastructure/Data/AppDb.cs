using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<BlackList> BlackLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRole)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRole)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin" },
                new Role { Id = 2, RoleName = "Staff" }
            );
            var hasher = new PasswordHasher<User>();
            var hashedAlice = hasher.HashPassword(null!, "AlicePassword123");
            var hashedBob = hasher.HashPassword(null!, "BobPassword123");

            // --- Seeding Users ---
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Alice", Password = "hashedpass1", RefreshToken = "" },
                new User { Id = 2, Username = "Bob", Password = "hashedpass2", RefreshToken = "" }
            );

            // --- Seeding Profiles ---
            modelBuilder.Entity<Profile>().HasData(
                new Profile { Id = 1, UserId = 1, Email = "alice@example.com" },
                new Profile { Id = 2, UserId = 2, Email = "bob@example.com" }
            );

            // --- Seeding UserRoles ---
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // Alice -> Admin
                new UserRole { UserId = 2, RoleId = 2 }  // Bob -> Staff
            );
        }


    }

}
