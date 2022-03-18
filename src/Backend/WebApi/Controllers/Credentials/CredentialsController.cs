﻿using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeeControl.Backend.Application.BoundContexts.Credentials.Commands;
using WeeControl.Backend.Application.BoundContexts.Credentials.Queries;
using WeeControl.Backend.Application.Interfaces;
using WeeControl.Common.BoundedContext.Credentials.DataTransferObjects;
using WeeControl.Common.BoundedContext.Credentials.Operations;
using WeeControl.Common.BoundedContext.RequestsResponses;

namespace WeeControl.Backend.WebApi.Controllers.Credentials
{
    [ApiController]
    [Route(ApiRouteLink.Route)]
    public class CredentialsController : Controller
    {
        private readonly IMediator mediatR;
        private readonly ICurrentUserInfo currentUserInfo;

        public CredentialsController(IMediator mediatR, ICurrentUserInfo currentUserInfo)
        {
            this.mediatR = mediatR;
            this.currentUserInfo = currentUserInfo;
        }

        [AllowAnonymous]
        [HttpPost(ApiRouteLink.Register.EndPoint)]
        [MapToApiVersion(ApiRouteLink.Register.Version)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ResponseDto<TokenDto>>> RegisterV1([FromBody] RequestDto<RegisterDto> dto)
        {
            if (dto.Payload is null)
                return BadRequest();

            var command = new RegisterCommand(dto, dto.Payload);
            var response = await mediatR.Send(command);

            return Ok(new ResponseDto<TokenDto>(response));

        }

        [AllowAnonymous]
        [HttpPost(ApiRouteLink.Login.EndPoint)]
        [MapToApiVersion(ApiRouteLink.Login.Version)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ResponseDto<TokenDto>>> LoginV1([FromBody] RequestDto<LoginDto> dto)
        {
            if (dto.Payload is null)
                return BadRequest();

            var query = new GetNewTokenQuery(dto, dto.Payload);
            var response = await mediatR.Send(query);

            return Ok(response);
        }

        /// <summary>
        ///     Used to get token which will be used to authorize user, device must match the same which had the temporary token.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut(ApiRouteLink.RequestRefreshToken.EndPoint)]
        [MapToApiVersion(ApiRouteLink.Login.Version)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseDto<TokenDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult<ResponseDto<TokenDto>>> RefreshTokenV1([FromBody] RequestDto dto)
        {
            var query = new GetNewTokenQuery(dto, currentUserInfo.GetSessionId());
            var response = await mediatR.Send(query);

            return Ok(response);
        }

    }
}