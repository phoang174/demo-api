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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = CheckUsernamePassword(request.username, request.password);
            if (user == null) return Result<LoginResult>.Fail(LoginError.WrongUsernamOrPassword);


            var refreshToken = _jwtService.GenerateRefreshToken(user);
            var accessToken = _jwtService.GenerateAccessToken(user);

            user.RefreshToken = refreshToken;
            await _userRepository.UpdateAsync(user);
            var roles = await this._userRepository.GetUserRolesAsync(user.Id);
            var res = new LoginResult
            {

                User = new UserResponseDto { Id = user.Id, Username = user.Username, Roles = roles },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return  Result<LoginResult>.Ok(res);
        }
        private User? CheckUsernamePassword(string username, string password)
        {
            var user = _userRepository.GetByUsernameAsync(username).Result;
            if (user == null) return null;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }
        public static class LoginError
        {
            public static Error WrongUsernamOrPassword = new("LoginCommandHandler.WrongUsernamOrPassword", "WrongUsernamOrPassword", 400);
        }
    }
}
