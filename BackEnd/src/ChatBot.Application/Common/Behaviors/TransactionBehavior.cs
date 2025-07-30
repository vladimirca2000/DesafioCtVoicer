using MediatR;
using Microsoft.Extensions.Logging;
using ChatBot.Application.Common.Interfaces;

namespace ChatBot.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Only wrap commands in transactions (not queries)
        if (!IsCommand())
        {
            return await next();
        }

        _logger.LogInformation("[TRANSACTION START] {RequestName}", requestName);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("[TRANSACTION COMMITTED] {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _logger.LogError(ex, "[TRANSACTION ROLLBACK] {RequestName}", requestName);

            throw;
        }
    }

    private static bool IsCommand()
    {
        return typeof(TRequest).Name.EndsWith("Command");
    }
}