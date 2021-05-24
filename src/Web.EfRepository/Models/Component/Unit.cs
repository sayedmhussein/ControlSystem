﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MySystem.Shared.Library.Dbos;
using MySystem.Web.EfRepository.Models.Basic;
using static MySystem.Shared.Library.Base.UnitBase;

namespace MySystem.Web.EfRepository.Models.Component
{
    internal static class Unit
    {
        static internal void CreateModelBuilder(DbContext dbContext, ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitDbo>().ToTable(nameof(Unit), nameof(Component));
            modelBuilder.Entity<UnitDbo>().HasIndex(x => x.UnitNo).IsUnique(true);
            modelBuilder.Entity<UnitDbo>().HasIndex(x => x.UnitType).IsUnique(false);
            modelBuilder.Entity<UnitDbo>().HasComment("__");

            if (dbContext.Database.IsNpgsql())
            {
                modelBuilder.HasPostgresExtension("uuid-ossp")
                .Entity<UnitDbo>()
                .Property(p => p.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            }
            else
            {
                modelBuilder.Entity<UnitDbo>().Property(p => p.Id).ValueGeneratedOnAdd();
            }
        }

        #region ef_functions
        static internal List<UnitDbo> GetUnitList(Guid buildingId)
        {
            return new()
            {
                new UnitDbo() { UnitNo = "88NB1234", UnitType = Types.Elevator, BuildingId = buildingId },
                new UnitDbo() { UnitNo = "88NE1234", UnitType = Types.Elevator, BuildingId = buildingId },
                new UnitDbo() { UnitNo = "88KE1234", UnitType = Types.Dumbwaiter, BuildingId = buildingId },
                new UnitDbo() { UnitNo = "88KC1234", UnitType = Types.Travellator, BuildingId = buildingId },
                new UnitDbo() { UnitNo = "88SH1234", UnitType = Types.Elevator, BuildingId = buildingId },
                new UnitDbo() { UnitNo = "88MT1234", UnitType = Types.Escalator, BuildingId = buildingId }
            };
        }

        
        #endregion
    }
}