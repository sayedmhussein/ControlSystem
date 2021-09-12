﻿using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WeeControl.Backend.Application.Common.Interfaces
{
    /// <summary>
    /// Use it to get the current user's (requester) session-id, claims and territories.
    /// </summary>
    public interface ICurrentUserInfo
    {
        Guid? SessionId { get; }

        IEnumerable<Guid> Territories { get; }

        IEnumerable<Claim> Claims { get; }
    }
}