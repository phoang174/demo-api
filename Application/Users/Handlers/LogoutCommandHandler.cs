using Application.Users.Commands;
using Domain.Entity;
using Domain.IRepository;
using MediatR;


namespace Application.Users.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand,bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlackListRepository _blackListRepository;
        public LogoutCommandHandler(IUserRepository userRepository, IBlackListRepository blackListRepository)
        {
            _userRepository = userRepository;
            _blackListRepository = blackListRepository;
        }
        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var existUser = await this._userRepository.GetByIdAsync(request.userId);
            if (existUser == null) return false;

            await this._blackListRepository.AddAsync(new BlackList
            {
                AccessToken = request.accessToken,
                RevokedAt = DateTime.UtcNow,
            });

            existUser.RefreshToken = "";
            await this._userRepository.UpdateAsync(existUser);

            return true;
        }
    }
}
