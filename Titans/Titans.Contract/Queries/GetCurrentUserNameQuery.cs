﻿using MediatR;

namespace Titans.Contract.Queries
{
    public record GetCurrentUserNameQuery : IRequest<string> { }
}
