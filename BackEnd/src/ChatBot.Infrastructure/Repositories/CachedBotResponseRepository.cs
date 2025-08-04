using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ChatBot.Infrastructure.Repositories;

/// <summary>
/// Decorator para IBotResponseRepository que adiciona funcionalidade de cache.
/// </summary>
public class CachedBotResponseRepository : IBotResponseRepository
{
    private readonly IBotResponseRepository _decoratedRepository; // O repositório original (concreto)
    private readonly ICacheService _cacheService;
    private const string AllBotResponsesCacheKey = "AllBotResponses"; // Chave para cache de todas as respostas

    public CachedBotResponseRepository(IBotResponseRepository decoratedRepository, ICacheService cacheService)
    {
        _decoratedRepository = decoratedRepository;
        _cacheService = cacheService;
    }

    public async Task<BotResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"BotResponse_{id}";
        var cachedItem = await _cacheService.GetAsync<BotResponse>(cacheKey);
        if (cachedItem != null)
        {
            return cachedItem;
        }

        var item = await _decoratedRepository.GetByIdAsync(id, cancellationToken);
        if (item != null)
        {
            await _cacheService.SetAsync(cacheKey, item, TimeSpan.FromMinutes(30)); // Cache por 30 minutos
        }
        return item;
    }

    public async Task<IEnumerable<BotResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cachedItems = await _cacheService.GetAsync<List<BotResponse>>(AllBotResponsesCacheKey);
        if (cachedItems != null)
        {
            return cachedItems;
        }

        var items = await _decoratedRepository.GetAllAsync(cancellationToken);
        await _cacheService.SetAsync(AllBotResponsesCacheKey, items.ToList(), TimeSpan.FromMinutes(30)); // Cache por 30 minutos
        return items;
    }

    public async Task AddAsync(BotResponse entity, CancellationToken cancellationToken = default)
    {
        await _decoratedRepository.AddAsync(entity, cancellationToken);
        await _cacheService.RemoveAsync(AllBotResponsesCacheKey); // Invalida o cache de todas as respostas
    }

    public async Task UpdateAsync(BotResponse entity, CancellationToken cancellationToken = default)
    {
        await _decoratedRepository.UpdateAsync(entity, cancellationToken);
        await _cacheService.RemoveAsync(AllBotResponsesCacheKey); // Invalida o cache de todas as respostas
        await _cacheService.RemoveAsync($"BotResponse_{entity.Id}"); // Invalida o cache do item específico
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _decoratedRepository.DeleteAsync(id, cancellationToken);
        await _cacheService.RemoveAsync(AllBotResponsesCacheKey); // Invalida o cache de todas as respostas
        await _cacheService.RemoveAsync($"BotResponse_{id}"); // Invalida o cache do item específico
    }

    public async Task RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _decoratedRepository.RestoreAsync(id, cancellationToken);
        await _cacheService.RemoveAsync(AllBotResponsesCacheKey); // Invalida o cache de todas as respostas
        await _cacheService.RemoveAsync($"BotResponse_{id}"); // Invalida o cache do item específico
    }
}