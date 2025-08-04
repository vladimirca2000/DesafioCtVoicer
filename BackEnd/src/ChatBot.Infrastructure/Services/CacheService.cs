using ChatBot.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // Usa IDistributedCache
using System.Text.Json; // Para serialização/deserialização
using System.Threading.Tasks;
using System;

namespace ChatBot.Infrastructure.Services;

/// <summary>
/// Implementação concreta do serviço de cache usando IDistributedCache.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedValue = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedValue))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }
        else
        {
            // Expiração padrão se nenhuma for especificada (ex: 5 minutos)
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        }

        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        await _cache.SetStringAsync(key, serializedValue, options);
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }
}