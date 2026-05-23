namespace Triumph.HealthMs.ExternalServices.CachingServices;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpiry = null,
        TimeSpan? slidingExpiry = null,
        CancellationToken ct = default);

    Task RemoveAsync(string key, CancellationToken ct = default);

    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        TimeSpan? absoluteExpiry = null,
        CancellationToken ct = default);
}