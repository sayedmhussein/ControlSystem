﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeeControl.Server.Application.Common.Interfaces;
using WeeControl.Server.Domain.BasicDbos.Territory;
using Xunit;

namespace WeeControl.Server.Persistence.Test.Territory
{
    public class TerritoryDboTests : IDisposable
    {
        private IMySystemDbContext context;

        public TerritoryDboTests()
        {
            context = new ServiceCollection().AddPersistenceAsInMemory(null).BuildServiceProvider().GetService<IMySystemDbContext>();
        }

        public void Dispose()
        {
            context = null;
        }

        [Fact]
        public async void WhenQueryingTerritories_ListNotEmpty()
        {
            var territories = await context.Territories.ToListAsync();

            Assert.NotEmpty(territories);
        }

        [Fact]
        public async void WhenAddingTerritory_IdHasNewId()
        {
            var territory = new TerritoryDbo() { CountryId = "aaa", Name = "Name" };

            await context.Territories.AddAsync(territory);
            await context.SaveChangesAsync(default);

            Assert.NotEqual(Guid.Empty, territory.Id);
        }

        

        
    }
}