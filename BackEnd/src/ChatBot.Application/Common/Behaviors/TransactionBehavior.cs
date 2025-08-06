using MediatR;
using Microsoft.Extensions.Logging;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;
using ChatBot.Domain.Entities;

namespace ChatBot.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (request is not ICommand)
        {
            return await next();
        }

        _logger.LogInformation("[TRANSACTION START] {RequestName}", requestName);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            var domainEvents = _unitOfWork.GetDomainEvents();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("[TRANSACTION COMMITTED] {RequestName}", requestName);

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            _unitOfWork.ClearDomainEvents();

            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "[TRANSACTION ROLLBACK] {RequestName}", requestName);
            throw;
        }
    }
}