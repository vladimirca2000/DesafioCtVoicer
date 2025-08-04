using System;
using System.Threading.Tasks;

namespace ChatBot.Application.Common.Interfaces;

/// <summary>
/// Contrato para um serviço genérico de cache.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Tenta obter um item do cache pela chave.
    /// </summary>
    /// <typeparam name="T">Tipo do item a ser recuperado.</typeparam>
    /// <param name="key">Chave do item no cache.</param>
    /// <returns>O item do cache se encontrado; caso contrário, o valor padrão do tipo.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Armazena um item no cache.
    /// </summary>
    /// <typeparam name="T">Tipo do item a ser armazenado.</typeparam>
    /// <param name="key">Chave para armazenar o item.</param>
    /// <param name="value">Valor a ser armazenado.</param>
    /// <param name="expiration">Tempo de expiração do item no cache (opcional).</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Remove um item do cache pela chave.
    /// </summary>
    /// <param name="key">Chave do item a ser removido.</param>
    Task RemoveAsync(string key);
}