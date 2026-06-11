namespace Triumph.HealthMs.ExternalServices.CachingServices;

public sealed class HybridCacheService(HybridCache hybridCache, ILogger<HybridCacheService> logger) : ICacheService
{
    private static readonly HybridCacheEntryOptions DefaultOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2) // L1 TTL
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync<T?>(
                key,
                _ => ValueTask.FromResult(default(T)),
                DefaultOptions,
                cancellationToken: ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting cache entry");
            return default(T);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        TimeSpan? slidingExpiry = null, CancellationToken ct = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = absoluteExpiry ?? DefaultOptions.Expiration,
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };

        try
        {
            await hybridCache.SetAsync(key, value, options, cancellationToken: ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error setting cache entry");
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await hybridCache.RemoveAsync(key, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error removing cache entry");
        }
    }

    public async Task<T> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, ValueTask<T>> factory,
        TimeSpan? absoluteExpiry = null, CancellationToken ct = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = absoluteExpiry ?? DefaultOptions.Expiration,
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };

        try
        {
            return await hybridCache.GetOrCreateAsync(key, factory, options, cancellationToken: ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting or creating cache entry");
            return await factory(ct);
        }
    }
}