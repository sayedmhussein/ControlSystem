﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MySystem.Shared.Library.Base;

namespace MySystem.Shared.Library.Dbo.Entity
{
    public class BuildingDbo : BuildingBase, IEntityDbo
    {
        public static IEnumerable<BuildingDbo> InitializeList()
        {
            var ho = new BuildingDbo() { Id = Guid.NewGuid(), CountryId = "SAU", BuildingName = "Saudi Arabia Head Office" };
            return new List<BuildingDbo>
            {
                ho,
                new BuildingDbo("Jeddah", "SAU")
            };
        }

        [Key]
        public Guid Id { get; set; }

        public BuildingDbo()
        {
        }

        public BuildingDbo(string name, string country) : this()
        {
            BuildingName = name;
            CountryId = country;
        }
    }
}
