using Application.Dtos;
using Application.Results;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Entity;
using Domain.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.userId);
            if (user == null) {
                return Result.Fail(DeleteUserError.UserNotFound);
            }
            await _userRepository.DeleteAsync(request.userId);
            return Result.Ok();
        }
    }
    public static class DeleteUserError
    {
        public static Error UserNotFound = new("DeleteUserCommandHandler.UserNotFound", "User Not Found", 400);
    }
}
