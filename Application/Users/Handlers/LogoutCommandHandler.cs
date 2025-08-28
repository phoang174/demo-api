using Application.Results;
using Application.Users.Commands;
using Domain.Entity;
using Domain.IRepository;
using MediatR;


namespace Application.Users.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlackListRepository _blackListRepository;
        public LogoutCommandHandler(IUserRepository userRepository, IBlackListRepository blackListRepository)
        {
            _userRepository = userRepository;
            _blackListRepository = blackListRepository;
        }
        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var existUser = await this._userRepository.GetByIdAsync(request.userId);
            if (existUser == null) return Result.Fail(LogoutError.NotfoundError);

            await this._blackListRepository.AddAsync(new BlackList
            {
                AccessToken = request.accessToken,
                RevokedAt = DateTime.UtcNow,
            });

            existUser.RefreshToken = "";
            await this._userRepository.UpdateAsync(existUser);

            return Result.Ok();
        }
    }
    public static class LogoutError
    {
        public static Error NotfoundError = new("LogoutCommandHandler.Notfound", "User not found", 400);
    } 
}
