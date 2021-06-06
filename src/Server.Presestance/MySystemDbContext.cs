﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySystem.Application.Common.Interfaces;
using MySystem.Domain.EntityDbo.EmployeeSchema;
using MySystem.Domain.EntityDbo.Territory;
using MySystem.SharedKernel.Enumerators;
using MySystem.SharedKernel.Interfaces;
using MySystem.SharedKernel.Services;

namespace MySystem.Persistence
{
    public class MySystemDbContext : DbContext, IMySystemDbContext
    {
        internal static DatabaseFacade DbFacade { get; private set; }

        public MySystemDbContext(DbContextOptions<MySystemDbContext> options) : base(options)
        {
            DbFacade = Database;

            //Database.EnsureDeleted(); //During Initial Development Only
            Database.EnsureCreated(); //During Initial Development Only

            if (Territories.Any() == false)
            {
                AddSuperUser();
            }
        }

        //Territory Schema
        public DbSet<TerritoryDbo> Territories { get; set; }

        //Employee Schema
        public DbSet<EmployeeDbo> Employees { get; set; }
        //
        public DbSet<EmployeeClaimDbo> EmployeeClaims { get; set; }
        public DbSet<EmployeeIdentityDbo> EmployeeIdentities { get; set; }
        //
        public DbSet<EmployeeSessionDbo> EmployeeSessions { get; set; }
        public DbSet<EmployeeSessionLogDbo> EmployeeSessionLogs { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.IsNpgsql())
            {
                modelBuilder.HasPostgresExtension("uuid-ossp");
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MySystemDbContext).Assembly);
        }

        private void AddSuperUser()
        {
            ISharedValues values = new SharedValues();

            var territory = new TerritoryDbo()
            {
                CountryId = values.Country[CountryEnum.USA],
                Name = "Head Office in USA"
            };
            Territories.Add(territory);
            SaveChanges();

            var superuser = new EmployeeDbo()
            {
                EmployeeTitle = values.PersonTitle[PersonalTitleEnum.Mr],
                FirstName = "Admin",
                LastName = "Admin",
                Gender = values.PersonGender[PersonalGenderEnum.Male],
                TerritoryId = territory.Id,
                Username = "admin",
                Password = "admin"
            };
            Employees.Add(superuser);
            SaveChanges();

            var superuserclaim = new EmployeeClaimDbo()
            {
                Employee = superuser,
                GrantedById = superuser.Id,
                ClaimType = values.ClaimType[ClaimTypeEnum.HumanResources],
                ClaimValue = string.Join(";", values.ClaimTag.Values)
            };
            EmployeeClaims.Add(superuserclaim);
            SaveChanges();
        }
    }
}
