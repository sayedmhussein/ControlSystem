﻿using System;
namespace MySystem.Shared.Library.Base
{
    public class UserClaimBase
    {
        public Guid EmployeeId { get; set; }
        
        public DateTime GrantedTs { get; set; }

        public DateTime? RevokedTs { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}
