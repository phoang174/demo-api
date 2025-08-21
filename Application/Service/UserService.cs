using Application.Dtos;
using Domain.Entity;
using Domain.IRepository;
using Microsoft.AspNetCore.Identity;
using Application.IService;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IRoleRepository _roleRepository;

        private readonly IUserRepository _userRepository;
        private readonly IBlackListRepository _blackList;
        private readonly IJwtService _jwt;

        public UserService(
            IUserRepository userRepository, 
            IJwtService jwt, 
            IBlackListRepository blackList,
            IRoleRepository roleRepository
            )
        {
            _userRepository = userRepository;
            _jwt = jwt;
            _blackList = blackList;
            _roleRepository = roleRepository;
        }

        public User createUser(string username, string password)
        {
            var user = new User { Username = username };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, password);

            return _userRepository.AddAsync(user).Result;
        }

        private User? CheckUsernamePassword(string username, string password)
        {
            var user = _userRepository.GetByUsernameAsync(username).Result;
            if (user == null) return null;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            var user = CheckUsernamePassword(username, password);
            if (user == null) return null;

            var refreshToken = _jwt.GenerateRefreshToken(user);
            var accessToken = _jwt.GenerateAccessToken(user);

            user.RefreshToken = refreshToken;
            await _userRepository.UpdateAsync(user);
            var roles = await this.getUserRole(user.Id);
            return new LoginResult
            {
                
                User = new UserResponseDto { Id = user.Id, Username = user.Username,Roles=roles },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResult> RefreshTokenHandler(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null) return null;

            var newRefreshToken = _jwt.GenerateRefreshToken(user);
            var accessToken = _jwt.GenerateAccessToken(user);

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user);

            return new LoginResult
            {
                User = new UserResponseDto { Id = user.Id, Username = user.Username },
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<List<string>> getUserRole(int userId)
        {
            var result=await _userRepository.GetUserRolesAsync(userId);
            return result;
        }

        public async Task<bool> LogOutAsync(int userId, string accessToken)
        {
            var existUser = await this._userRepository.GetByIdAsync(userId);
            if (existUser == null) return false;

            await this._blackList.AddAsync(new BlackList
            {
                AccessToken = accessToken,
                RevokedAt = DateTime.UtcNow,
            });

            existUser.RefreshToken = "";
            await this._userRepository.UpdateAsync(existUser);

            return true;
        }

        public async Task<List<UserProfile>> GetAllUserProfiles()
        {
            var users = await this._userRepository.GetAllItemAsync();

            var result = new List<UserProfile>();

            foreach (var user in users)
            {

                if (user.Profile != null)
                {
                    var Roles =await this.getUserRole(user.Id);
                    var temp = new UserProfile
                    {
                        Username = user.Username,
                        Birthday = user.Profile.Birthday,
                        Email = user.Profile.Email,
                        UserId = user.Id,
                        Roles = Roles
                    };

                    result.Add(temp);
                }
            }
    
            return result;
        }
        public async Task<UserProfile> CreateUserProfile(CreateProfileDto createProfileDto)
        {

            var user = new User
            {
                Username = createProfileDto.Username,
                RefreshToken="",
                Profile = new Profile
                {
                    Birthday = createProfileDto.Birthday,
                    Email = createProfileDto.Email,
                },
                
            };
            foreach (var roleId in createProfileDto.Roles)
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
                Roles=roles
            };
        }

        public async Task<UserProfile> UpdateUser(UpdateUserDto updateUserDto)
        {
            var user = await this._userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null) throw new Exception("User not found");

            if (user.Profile == null)
            {
                user.Profile = new Profile();
            }
            user.Username = updateUserDto.Username;
            user.Profile.Birthday = updateUserDto.Birthday;
            user.Profile.Email = updateUserDto.Email;
            var existingRoles = user.UserRole.Select(ur => ur.RoleId).ToList();
            var rolesToAdd = updateUserDto.Roles.Except(existingRoles).ToList();
            var rolesToRemove = existingRoles.Except(updateUserDto.Roles).ToList();

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

        public async Task DeleteUser(int userId)
        {
     
            await _userRepository.DeleteAsync(userId);

        }

    }
}
