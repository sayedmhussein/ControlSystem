﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeeControl.Backend.Domain.BoundedContexts.Credentials.DatabaseObjects;
using WeeControl.Backend.Domain.Credentials.DatabaseObjects;

namespace WeeControl.Backend.Persistence.Credentials.Configurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserDbo>
    {
        public void Configure(EntityTypeBuilder<UserDbo> builder)
        {
            builder.ToTable("user", "credentials");
            builder.HasKey(p => p.UserId);
            builder.Property(p => p.UserId).ValueGeneratedOnAdd();

            builder.HasMany(x => x.Claims).WithOne().HasForeignKey(x => x.UserId);

            
        }
    }
}