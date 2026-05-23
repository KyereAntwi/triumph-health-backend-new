namespace Triumph.HealthMs.ExternalServices.CachingServices;

public sealed class HybridCacheService(HybridCache hybridCache) : ICacheService
{
    private static readonly HybridCacheEntryOptions DefaultOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2) // L1 TTL
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        // HybridCache doesn't expose a pure get, so we use a no-op factory
        // that signals a miss by returning default — track misses via a sentinel if needed
        return await hybridCache.GetOrCreateAsync<T?>(
            key,
            _ => ValueTask.FromResult(default(T)),
            DefaultOptions,
            cancellationToken: ct);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        TimeSpan? slidingExpiry = null, CancellationToken ct = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = absoluteExpiry ?? DefaultOptions.Expiration,
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };
        await hybridCache.SetAsync(key, value, options, cancellationToken: ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await hybridCache.RemoveAsync(key, ct);

    public async Task<T> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, ValueTask<T>> factory,
        TimeSpan? absoluteExpiry = null, CancellationToken ct = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = absoluteExpiry ?? DefaultOptions.Expiration,
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };

        return await hybridCache.GetOrCreateAsync(key, factory, options, cancellationToken: ct);
    }
}