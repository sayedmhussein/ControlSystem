﻿using Microsoft.EntityFrameworkCore;
using WeeControl.Core.Domain.Contexts.Business;
using WeeControl.Core.Domain.Contexts.Business.Elevator;
using WeeControl.Core.Domain.Contexts.User;
using CustomerDbo = WeeControl.Core.Domain.Contexts.User.CustomerDbo;

namespace WeeControl.Core.Domain.Interfaces;

public interface IEssentialDbContext : IDbContext
{
    DbSet<PersonDbo> Person { get; set; }
    DbSet<UserDbo> Users { get; set; }
    DbSet<PersonIdentity> UserIdentities { get; set; }
    DbSet<UserNotificationDbo> UserNotifications { get; set; }
    DbSet<UserSessionDbo> UserSessions { get; set; }
    DbSet<UserSessionLogDbo> SessionLogs { get; set; }
    DbSet<TerritoryDbo> Territories { get; set; }
    DbSet<UserClaimDbo> UserClaims { get; set; }
    DbSet<EmployeeDbo> Employees { get; set; }
    DbSet<CustomerDbo> Customers { get; set; }
}