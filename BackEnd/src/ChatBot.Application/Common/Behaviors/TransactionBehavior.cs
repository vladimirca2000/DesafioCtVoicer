using MediatR;
using Microsoft.Extensions.Logging;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Interfaces; // Necessário para IDomainEvent
using System.Linq;
using System.Collections.Generic;
using ChatBot.Domain.Entities; // Necessário para BaseEntity

namespace ChatBot.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator; // Injetar IMediator para publicar eventos

    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mediator = mediator; // Atribuir IMediator
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Apenas comandos que herdam de ICommand (que altera estado) são envolvidos em transações
        // e seus eventos de domínio serão publicados.
        // Queries não devem ter eventos de domínio, pois não alteram estado.
        if (request is not ICommand) // Usar 'is not ICommand' para filtrar comandos
        {
            return await next();
        }

        _logger.LogInformation("[TRANSACTION START] {RequestName}", requestName);

        // Inicia a transação de banco de dados
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Executa o próximo handler no pipeline (que pode salvar entidades e adicionar eventos de domínio)
            var response = await next();

            // Pega todos os eventos de domínio das entidades que foram rastreadas e modificadas nesta transação
            // ATENÇÃO: Isso exige que o IUnitOfWork (ou o DbContext) tenha acesso às entidades rastreadas
            // e seus DomainEvents. Uma forma comum é expor um método GetDomainEvents() no IUnitOfWork
            // que acesse o ChangeTracker do DbContext.

            // Por simplicidade e para fins de demonstração, se o IUnitOfWork não expõe isso,
            // poderíamos argumentar que os eventos de domínio deveriam ser coletados
            // PELO HANDLER e passados para o UnitOfWork, ou que o DbContext deveria publicá-los
            // em seu SaveChangesAsync (como discutido anteriormente, com ressalvas).

            // ASSUMINDO que o IUnitOfWork tem um mecanismo para obter os DomainEvents
            // ou que os eventos são adicionados à entidade e "coletados" aqui,
            // ou que o SaveChangesAsync do DbContext já os publicaria.
            // Para este cenário, vamos seguir a ideia de que o SaveChangesAsync do UnitOfWork
            // (que encapsula o DbContext) fará a publicação.
            // No entanto, para garantir a publicação de eventos *APÓS* o commit da transação,
            // precisamos de uma abordagem um pouco diferente.

            // Uma abordagem mais robusta seria:
            // 1. O SaveChangesAsync do UnitOfWork/DbContext coleta os eventos.
            // 2. Ele os armazena temporariamente (ex: em uma lista estática de eventos para publicar).
            // 3. O TransactionBehavior publica esses eventos APENAS SE a transação for bem-sucedida.

            // Para manter a implementação simples e funcional com a estrutura atual:
            // Vamos assumir que SaveChangesAsync do UnitOfWork irá persistir as entidades e que os DomainEvents
            // que foram *adicionados* às entidades rastreadas estão disponíveis para serem coletados *antes*
            // do commit final.

            // Coleta os eventos de domínio das entidades rastreadas no UnitOfWork ANTES do SaveChanges
            var domainEvents = _unitOfWork.GetDomainEvents(); // Este método precisa ser implementado/exposto no IUnitOfWork

            // Salva as mudanças no banco de dados
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Comita a transação de banco de dados
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("[TRANSACTION COMMITTED] {RequestName}", requestName);

            // Publica os eventos de domínio APÓS o commit da transação
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            // Limpa os eventos das entidades após a publicação (se as entidades ainda estiverem rastreadas)
            _unitOfWork.ClearDomainEvents(); // Este método precisa ser implementado/exposto no IUnitOfWork

            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "[TRANSACTION ROLLBACK] {RequestName}", requestName);
            throw;
        }
    }

    // Método auxiliar (não faz parte do TransactionBehavior diretamente, mas representa a necessidade)
    // private static bool IsCommand()
    // {
    //     return typeof(TRequest).Name.EndsWith("Command"); // Já substituído por 'is ICommand'
    // }
}