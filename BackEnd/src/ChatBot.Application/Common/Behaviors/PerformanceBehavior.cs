﻿using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ChatBot.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // Log if request takes more than 500ms
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogWarning("[PERFORMANCE] {RequestName} took {ElapsedMilliseconds} milliseconds {@Request}",
                requestName, elapsedMilliseconds, request);
        }

        return response;
    }
}