using DistributedSystem.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace DistributedSystem.Infrastructure.Caching;
public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private static ConcurrentDictionary<string, bool> cacheKeys = new ConcurrentDictionary<string, bool>();
    
    public CacheService(IDistributedCache distributedCache) 
    {  
        _distributedCache = distributedCache; 
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (cacheValue is null) return null;

        T? value = JsonConvert.DeserializeObject<T>(cacheValue);
        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
        cacheKeys.TryRemove(key, out _);
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        IEnumerable<Task> tasks = cacheKeys.Where(o=>o.Key.StartsWith(prefixKey)).Select(o=> RemoveAsync(o.Key, cancellationToken));

        await Task.WhenAll(tasks); //execute in parallel 
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        string cacheValue = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);
        cacheKeys.TryAdd(key, true);
    }
}
