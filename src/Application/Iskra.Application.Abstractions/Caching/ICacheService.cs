namespace Iskra.Application.Abstractions.Caching;

/// <summary>
/// Defines a contract for caching operations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets an item from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached item, or default if not found.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Asynchronously gets an item from the cache, or creates it if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">A function to create the item if not found.</param>
    /// <param name="options">Cache entry options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly created item.</returns>
    Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, 
        CacheEntryOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an item in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The item to cache.</param>
    /// <param name="options">Cache entry options.</param>
    void Set<T>(string key, T value, CacheEntryOptions? options = null);

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    void Remove(string key);
}