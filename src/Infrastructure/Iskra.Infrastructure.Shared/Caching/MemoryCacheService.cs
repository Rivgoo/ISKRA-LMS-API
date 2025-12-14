using Iskra.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Iskra.Infrastructure.Shared.Caching;

internal sealed class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public T? Get<T>(string key)
    {
        return memoryCache.TryGetValue(key, out T? value) ? value : default;
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await memoryCache.GetOrCreateAsync(key, async entry =>
        {
            ApplyOptions(entry, options);
            return await factory(cancellationToken);
        });
    }

    public void Set<T>(string key, T value, CacheEntryOptions? options = null)
    {
        using var entry = memoryCache.CreateEntry(key);
        entry.Value = value;
        ApplyOptions(entry, options);
    }

    private static void ApplyOptions(ICacheEntry entry, CacheEntryOptions? options)
    {
        options ??= CacheEntryOptions.Default;

        if (options.AbsoluteExpiration.HasValue)
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration;

        if (options.SlidingExpiration.HasValue)
            entry.SlidingExpiration = options.SlidingExpiration;
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }
}