using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeeControl.Application.EssentialContext.Commands;
using WeeControl.Application.EssentialContext.Queries;
using WeeControl.SharedKernel;
using WeeControl.SharedKernel.DataTransferObjects;
using WeeControl.SharedKernel.RequestsResponses;
using WeeControl.WebApi.Security.Policies;

namespace WeeControl.WebApi.Controllers.Essentials;

[ApiController]
[Authorize]
[ProducesResponseType((int)HttpStatusCode.OK)]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
[ProducesResponseType((int)HttpStatusCode.Forbidden)]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AdminController  : Controller
{
    private readonly IMediator mediator;

    public AdminController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Authorize(Policy = DeveloperWithDatabaseOperationPolicy.Name)]
    [HttpHead(Api.Essential.Admin.Base)]
    public async Task<ActionResult> PopulateDatabase()
    {
        await mediator.Send(new SeedEssentialDatabaseCommand());
        return Ok();
    }
    
    [HttpGet(Api.Essential.Admin.User)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ResponseDto<IEnumerable<UserDto>>>> GetListOfUsersV1()
    {
        var command = new GetListOfUsersQuery();
        var response = await mediator.Send(command);

        return Ok(response);
    }
    
    [HttpGet(Api.Essential.Admin.User + "/{username}")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ResponseDto<IEnumerable<UserDto>>>> GetUserDetailsV1(string username)
    {
        var command = new GetUserDetailsQuery(username);
        var response = await mediator.Send(command);

        return Ok(response);
    }
    
    [HttpGet(Api.Essential.Admin.Territory)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ResponseDto<IEnumerable<UserDto>>>> GetListOfTerritoriesV1()
    {
        var command = new GetListOfTerritoriesQuery();
        var response = await mediator.Send(command);

        return Ok(response);
    }
}