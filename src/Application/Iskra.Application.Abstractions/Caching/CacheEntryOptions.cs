namespace Iskra.Application.Abstractions.Caching;

public class CacheEntryOptions
{
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }

    public static CacheEntryOptions Default => new() { AbsoluteExpiration = TimeSpan.FromMinutes(10) };

    public static CacheEntryOptions Absolute(TimeSpan time) => new() { AbsoluteExpiration = time };
    public static CacheEntryOptions Sliding(TimeSpan time) => new() { SlidingExpiration = time };
}