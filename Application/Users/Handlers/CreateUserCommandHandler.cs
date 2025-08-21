using Application.Dtos;
using Application.IService;
using Application.Users.Commands;
using Domain.Entity;
using Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserProfile>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfile> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Username = request.Username,
                RefreshToken = "",
                Profile = new Profile
                {
                    Birthday = request.Birthday,
                    Email = request.Email,
                },

            };
            foreach (var roleId in request.Roles)
            {
                user.UserRole.Add(new UserRole
                {
                    RoleId = roleId,
                    User = user
                });
            }
            var hasher = new PasswordHasher<User>();
            var password = hasher.HashPassword(user, "123456"); ;
            user.Password = password;
            var result = await _userRepository.AddAsync(user);
            var userResult = await _userRepository.GetByUsernameAsync(user.Username);
            var roles = await _userRepository.GetUserRolesAsync(userResult.Id);
            return new UserProfile
            {
                UserId = result.Id,
                Username = result.Username,
                Birthday = result.Profile.Birthday,
                Email = result.Profile.Email,
                Roles = roles
            };
        }
    }
}
