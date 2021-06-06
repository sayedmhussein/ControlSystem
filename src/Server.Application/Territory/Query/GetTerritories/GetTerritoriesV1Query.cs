﻿using System;
using System.Collections.Generic;
using MediatR;
using MySystem.SharedKernel.Entities.Territory.V1Dto;

namespace MySystem.Application.Territory.Query.GetTerritories
{
    public class GetTerritoriesV1Query : IRequest<IEnumerable<TerritoryDto>>
    {
        public Guid? EmployeeId { get; set; }

        public Guid? SessionId { get; set; }

        public Guid? TerritoryId { get; set; }
    }
}