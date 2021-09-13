﻿using System;
using System.Collections.Generic;
using MediatR;
using WeeControl.SharedKernel.DtosV1.Territory;

namespace WeeControl.Backend.Application.EntityGroups.Territory.Queries.GetTerritoryV1
{
    public class GetTerritoriesQuery : IRequest<IEnumerable<IdentifiedTerritoryDto>>
    {
        public GetTerritoriesQuery(Guid? id)
        {
            TerritoryId = id;
        }

        public Guid? TerritoryId { get; set; }
    }
}