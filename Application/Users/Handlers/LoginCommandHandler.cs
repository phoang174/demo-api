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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = CheckUsernamePassword(request.username, request.password);
            if (user == null) return null;

            var refreshToken = _jwtService.GenerateRefreshToken(user);
            var accessToken = _jwtService.GenerateAccessToken(user);

            user.RefreshToken = refreshToken;
            await _userRepository.UpdateAsync(user);
            var roles = await this._userRepository.GetUserRolesAsync(user.Id);
            return new LoginResult
            {

                User = new UserResponseDto { Id = user.Id, Username = user.Username, Roles = roles },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        private User? CheckUsernamePassword(string username, string password)
        {
            var user = _userRepository.GetByUsernameAsync(username).Result;
            if (user == null) return null;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }

    }
}
