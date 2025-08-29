using Application.Dtos;
using Application.IService;
using Application.Results;
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
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserProfile>>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserProfile>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return Result<UserProfile>.Fail(CreateUserError.UsernameAlreadyExists);
            }

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
            user.Password = hasher.HashPassword(user, "123456");

            var result = await _userRepository.AddAsync(user);
            //var userResult = await _userRepository.GetByUsernameAsync(user.Username);
            var roles = await _userRepository.GetUserRolesAsync(result.Id);

            var response = new UserProfile
            {
                UserId = result.Id,
                Username = result.Username,
                Birthday = result.Profile.Birthday,
                Email = result.Profile.Email,
                Roles = roles
            };

            return Result<UserProfile>.Ok(response);
        }

    }
    public static class CreateUserError
    {
        public static Error UsernameAlreadyExists = new("CreateUserCommandHandler.UsernameAlreadyExists", "Username already exists", 400);
    }
}
