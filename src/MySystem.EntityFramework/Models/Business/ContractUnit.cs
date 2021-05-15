﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Sayed.MySystem.EntityFramework.Models.Component;
using Sayed.MySystem.Shared.Dbos;

namespace Sayed.MySystem.EntityFramework.Models.Business
{
    internal static class ContractUnit
    {
        static internal void CreateContractUnitModel(DbContext dbContext, ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractUnitDbo>().ToTable(nameof(ContractUnit), nameof(Business));
            modelBuilder.Entity<ContractUnitDbo>().HasIndex(x => new { x.ContractId, x.UnitId }).IsUnique(false);

            if (dbContext.Database.IsNpgsql())
            {
                modelBuilder.HasPostgresExtension("uuid-ossp")
                .Entity<ContractUnitDbo>()
                .Property(p => p.ContractUnitId)
                .HasDefaultValueSql("uuid_generate_v4()");
            }
            else
            {
                modelBuilder.Entity<ContractUnitDbo>().Property(p => p.ContractUnitId).ValueGeneratedOnAdd();
            }

            modelBuilder.Entity<ContractUnitDbo>().Property(p => p.ActivationTs).HasDefaultValue(DateTime.Now);
        }



        #region ef_functions
        static internal List<ContractUnitDbo> GetContractUnitList(Guid contractId, Guid unitId)
        {
            return new()
            {
                new ContractUnitDbo() { ContractId = contractId, UnitId = unitId }
            };
        }

        
        #endregion
    }
}
