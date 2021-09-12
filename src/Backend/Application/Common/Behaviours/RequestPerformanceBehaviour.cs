﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WeeControl.Backend.Application.Common.Interfaces;

namespace WeeControl.Backend.Application.Common.Behaviours
{
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserInfo _currentUserService;

        public RequestPerformanceBehaviour(ILogger<TRequest> logger, ICurrentUserInfo currentUserService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500)
            {
                var name = typeof(TRequest).Name;

                _logger.LogWarning("WeeControl Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                    name, _timer.ElapsedMilliseconds, _currentUserService.SessionId, request);
            }

            return response;
        }
    }
}