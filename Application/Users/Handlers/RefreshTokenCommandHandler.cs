using Application.Dtos;
using Application.IService;
using Application.Results;
using Application.Users.Commands;
using Domain.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        public async Task<Result<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.refreshToken);
            if (user == null) return Result<LoginResult>.Fail(RefreshTokenCommandError.UserNotFound);

            var newRefreshToken = _jwtService.GenerateRefreshToken(user);
            var accessToken = _jwtService.GenerateAccessToken(user);

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user);
            var roles = await this._userRepository.GetUserRolesAsync(user.Id);

            var result = new LoginResult
            {
                User = new UserResponseDto { Id = user.Id, Username = user.Username,Roles= roles },
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
            return Result<LoginResult>.Ok(result);
        }
        public static class RefreshTokenCommandError
        {
            public static Error UserNotFound = new("RefreshTokenCommandHandler.UserNotFound", "WrongUsernamOrPassword", 400);
        }
    }
}
