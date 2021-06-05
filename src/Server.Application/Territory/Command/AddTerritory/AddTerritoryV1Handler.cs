﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MySystem.Application.Common.Exceptions;
using MySystem.Application.Common.Interfaces;
using MySystem.Domain.EntityDbo.PublicSchema;
using MySystem.Domain.Extensions;
using MySystem.SharedKernel.Entities.Territory.V1Dto;

namespace MySystem.Application.Territory.Command.AddTerritory
{
    public class AddTerritoryV1Handler : IRequestHandler<AddTerritoryV1Command, IEnumerable<TerritoryDto>>
    {
        private readonly IMySystemDbContext context;

        public AddTerritoryV1Handler(IMySystemDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<TerritoryDto>> Handle(AddTerritoryV1Command request, CancellationToken cancellationToken)
        {
            var responses = new List<TerritoryDto>();

            if (request.TerritoryDtos is null || request.TerritoryDtos.Count == 0)
            {
                throw new BadRequestException("");
            }
            foreach (var dto in request.TerritoryDtos)
            {
                try
                {
                    var dbo = dto.ToDbo<TerritoryDto, TerritoryDbo>();

                    if (dto.Id == null || dto.Id == Guid.Empty)
                    {
                        await context.Territories.AddAsync(dbo, cancellationToken);
                    }
                    else
                    {
                        context.Territories.Update(dbo);
                    }

                    await context.SaveChangesAsync(cancellationToken);

                    responses.Add(dbo.ToDto<TerritoryDbo, TerritoryDto>());
                }
                catch
                {
                    //Log Please...
                }
            }

            return responses;
        }
    }
}
