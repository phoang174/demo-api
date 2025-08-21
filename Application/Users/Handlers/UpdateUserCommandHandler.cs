using Application.Dtos;
using Application.Users.Commands;
using Domain.Entity;
using Domain.IRepository;
using MediatR;


namespace Application.Users.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserProfile>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfile> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await this._userRepository.GetByIdAsync(request.Id);
            if (user == null) throw new Exception("User not found");

            if (user.Profile == null)
            {
                user.Profile = new Profile();
            }
            user.Username = request.Username;
            user.Profile.Birthday = request.Birthday;
            user.Profile.Email = request.Email;
            var existingRoles = user.UserRole.Select(ur => ur.RoleId).ToList();
            var rolesToAdd = request.Roles.Except(existingRoles).ToList();
            var rolesToRemove = existingRoles.Except(request.Roles).ToList();

            user.UserRole = user.UserRole
                .Where(ur => !rolesToRemove.Contains(ur.RoleId))
                .ToList();

            foreach (var roleId in rolesToAdd)
            {
                user.UserRole.Add(new UserRole { RoleId = roleId, UserId = user.Id });
            }
            await this._userRepository.UpdateAsync(user);
            user = await _userRepository.GetByIdAsync(user.Id);

            var roles = user.UserRole.Select(e => e.Role.RoleName).ToList();
            return new UserProfile
            {
                UserId = user.Id,
                Username = user.Username,
                Birthday = user.Profile.Birthday,
                Email = user.Profile.Email,
                Roles = roles
            };
        }
    }
}
