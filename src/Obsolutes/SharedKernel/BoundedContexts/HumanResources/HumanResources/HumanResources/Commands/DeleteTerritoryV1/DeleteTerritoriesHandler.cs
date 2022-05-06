﻿// using System;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using MediatR;
// using WeeControl.Backend.Application.Exceptions;
// using WeeControl.Backend.Application.Interfaces;
// using WeeControl.Backend.Domain.BoundedContexts.HumanResources;
// using WeeControl.Common.SharedKernel.EntityGroups.Employee.Interfaces;
// using WeeControl.Common.SharedKernel.EntityGroups.Territory.Interfaces;
//
// namespace WeeControl.Backend.Application.BoundContexts.HumanResources.Commands.DeleteTerritoryV1
// {
//     public class DeleteTerritoryHandler : IRequestHandler<DeleteTerritoryCommand>
//     {
//         private readonly IHumanResourcesDbContext context;
//         private readonly ICurrentUserInfo userInfo;
//         private readonly ITerritoryAttribute values;
//         private readonly IEmployeeAttribute employeeValues;
//
//         public DeleteTerritoryHandler(IHumanResourcesDbContext context, ICurrentUserInfo userInfo, ITerritoryAttribute values, IEmployeeAttribute employeeValues)
//         {
//             this.context = context ?? throw new ArgumentNullException("Db Context can't be Null!");
//             this.userInfo = userInfo ?? throw new ArgumentNullException("User Info can't be Null!");
//             this.values = values;
//             this.employeeValues = employeeValues;
//         }
//
//         public Task<Unit> Handle(DeleteTerritoryCommand request, CancellationToken cancellationToken)
//         {
//             if (request == null)
//             {
//                 throw new ArgumentNullException();
//             }
//
//             if (request.TerritoryId == Guid.Empty)
//             {
//                 throw new BadRequestException("");
//             }
//
//             //todo: refactor as below is very bad performance!
//             try
//             {
//                 var territory = context.Territories.FirstOrDefault(x => x.Id == request.TerritoryId);
//                 if (territory == null)
//                 {
//                     throw new NotFoundException("");
//                 }
//                 var depenantCount = context.Territories.Where(x => x.ReportToId == request.TerritoryId).Count();
//                 if (depenantCount > 0)
//                 {
//                     throw new DeleteFailureException("There are other territories which report to this territory!");
//                 }
//
//                 context.Territories.RemoveRange(territory);
//                 context.SaveChangesAsync(default).GetAwaiter().GetResult();
//             }
//             catch (NotFoundException)
//             {
//                 throw;
//             }
//             catch(Exception e)
//             {
//                 _ = e.Message;
//                 throw;
//                 throw new DeleteFailureException("");
//             }
//
//             return Task.FromResult(Unit.Value);
//         }
//     }
// }