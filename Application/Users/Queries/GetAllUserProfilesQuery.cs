﻿using Application.Dtos;
using Application.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public class GetAllUserProfilesQuery : IRequest<Result<List<UserProfile>>>
    {
    }
}
