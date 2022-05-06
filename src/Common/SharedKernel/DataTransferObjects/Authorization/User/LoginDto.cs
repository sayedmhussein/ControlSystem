﻿using System.ComponentModel.DataAnnotations;

namespace WeeControl.Common.SharedKernel.DataTransferObjects.Authorization.User
{
    public class LoginDto
    {
        [Required]
        [StringLength(45, ErrorMessage = "username cannot be longer than 45 characters.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required] public string Password { get; set; } = string.Empty;
    }
}