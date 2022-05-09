﻿using System;
using MediatR;
using WeeControl.Common.SharedKernel.DataTransferObjects.Authorization.User;
using WeeControl.Common.SharedKernel.Interfaces;
using WeeControl.Common.SharedKernel.RequestsResponses;

namespace WeeControl.Backend.Application.EssentialContext.Queries
{
    public class GetNewTokenQuery : IRequest<ResponseDto<TokenDto>>
    {
        public GetNewTokenQuery(IRequestDto request)
        {
            Request = request;
        }

        public GetNewTokenQuery(IRequestDto<LoginDto> dto)
        {
            Request = dto;
            Payload = new LoginDto(dto.Payload.UsernameOrEmail, dto.Payload.Password);
        }
        
        [Obsolete("Use other constructor which has one argument.")]
        public GetNewTokenQuery(RequestDto request, LoginDto payload)
        {
            Request = request;
            Payload = payload;
        }

        [Obsolete("Better pass request only then inject CurrentUserInfo in handler.")]
        public GetNewTokenQuery(RequestDto request, Guid? sessionId)
        {
            Request = request;
            SessionId = sessionId;
        }

        public IRequestDto Request { get; }
        public LoginDto Payload { get; }
        public Guid? SessionId { get; set; }
    }
}