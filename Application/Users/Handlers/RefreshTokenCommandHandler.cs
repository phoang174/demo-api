using Application.Dtos;
using Application.IService;
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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.refreshToken);
            if (user == null) return null;

            var newRefreshToken = _jwtService.GenerateRefreshToken(user);
            var accessToken = _jwtService.GenerateAccessToken(user);

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user);

            return new LoginResult
            {
                User = new UserResponseDto { Id = user.Id, Username = user.Username },
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
