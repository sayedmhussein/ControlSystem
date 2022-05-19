using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeeControl.Application.EssentialContext.Queries;
using WeeControl.SharedKernel.Essential;
using WeeControl.SharedKernel.Essential.RequestDTOs;
using WeeControl.SharedKernel.RequestsResponses;

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
    
    [HttpGet(Api.Essential.Admin.User)]
    [MapToApiVersion(RegisterDto.HttpPostMethod.Version)]
    public async Task<ActionResult<ResponseDto<IEnumerable<UserDto>>>> GetListOfUsersV1()
    {
        var command = new GetListOfUsersQuery();
        var response = await mediator.Send(command);

        return Ok(response);
    }
    
    [HttpGet(Api.Essential.Admin.User + "/{username}")]
    [MapToApiVersion(RegisterDto.HttpPostMethod.Version)]
    
    public async Task<ActionResult<ResponseDto<IEnumerable<UserDto>>>> GetUserDetailsV1(string username)
    {
        var command = new GetUserDetailsQuery(username);
        var response = await mediator.Send(command);

        return Ok(response);
    }
    
    [HttpGet(Api.Essential.Admin.Territory)]
    [MapToApiVersion(RegisterDto.HttpPostMethod.Version)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ResponseDto<IEnumerable<UserDto>>>> GetListOfTerritoriesV1()
    {
        var command = new GetListOfTerritoriesQuery();
        var response = await mediator.Send(command);

        return Ok(response);
    }
}