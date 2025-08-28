using Application.Dtos;
using Application.Results;
using Application.Users.Queries;
using Domain.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Handlers
{
    public class GetAllUserProfilesQueryHandler : IRequestHandler<GetAllUserProfilesQuery, Result<List<UserProfile>>>
    {
        private readonly IUserRepository _userRepository;
        public GetAllUserProfilesQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<List<UserProfile>>> Handle(GetAllUserProfilesQuery request, CancellationToken cancellationToken)
        {
            var users = await this._userRepository.GetAllItemAsync();

            var result = new List<UserProfile>();

            foreach (var user in users)
            {

                if (user.Profile != null)
                {
                    var Roles = await this._userRepository.GetUserRolesAsync(user.Id);
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

            return Result<List<UserProfile>>.Ok(result);
        }
    }
}
