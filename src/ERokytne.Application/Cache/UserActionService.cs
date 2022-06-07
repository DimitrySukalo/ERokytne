using EasyCaching.Core;

namespace ERokytne.Application.Cache;

public class UserActionService
{
    private readonly IHybridCachingProvider _cachingProvider;

    public UserActionService(IHybridCachingProvider cachingProvider)
    {
        _cachingProvider = cachingProvider;
    }

    public virtual async Task<T> GetUserCacheAsync<T>(string key, Func<Task<T>> dataRetriever)
    {
        var cacheValue = await _cachingProvider.GetAsync(
            key, dataRetriever, TimeSpan.FromHours(1));
            
        return cacheValue.Value;
    }

    public virtual async Task SetUserCacheAsync<T>(string key, T cache)
    {
        await _cachingProvider.SetAsync(key, cache, TimeSpan.FromDays(1));
    }

    public virtual async Task DeleteUserCacheAsync(string key)
    {
        await _cachingProvider.RemoveAsync(key);
    }
}