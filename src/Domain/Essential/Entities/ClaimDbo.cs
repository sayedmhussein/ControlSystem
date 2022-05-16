﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WeeControl.Domain.Essential.Entities;

public class ClaimDbo
{
    [Key]
    public Guid ClaimId { get; set; }

    public Guid? UserId { get; set; }
    //public UserDbo User { get; set; }

    [Required]
    [StringLength(5)]
    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

    public DateTime GrantedTs { get; set; }
    public UserDbo GrantedBy { get; set; }

    public DateTime? RevokedTs { get; set; }
    public UserDbo RevokedBy { get; set; }
}