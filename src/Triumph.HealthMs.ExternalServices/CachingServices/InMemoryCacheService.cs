namespace Triumph.HealthMs.ExternalServices.CachingServices;

public sealed class InMemoryCacheService(IMemoryCache cache) : ICacheService
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null,
        TimeSpan? slidingExpiry = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();

        if (absoluteExpiry.HasValue)
            options.AbsoluteExpirationRelativeToNow = absoluteExpiry;
        else
            options.AbsoluteExpirationRelativeToNow = DefaultExpiry;

        if (slidingExpiry.HasValue)
            options.SlidingExpiration = slidingExpiry;

        cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrCreateAsync<T>(string key,
        Func<CancellationToken, ValueTask<T>> factory,
        TimeSpan? absoluteExpiry = null, CancellationToken ct = default)
    {
        if (cache.TryGetValue(key, out T? cached) && cached is not null)
            return cached;

        var value = await factory(ct);
        await SetAsync(key, value, absoluteExpiry, null, ct);
        return value;
    }
}