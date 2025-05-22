
using AllTheBeans.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AllTheBeans.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out T value))
                {
                    _logger.LogDebug("[Cache] HIT for key: {Key}", key);
                    return Task.FromResult(value);
                }

                _logger.LogDebug("[Cache] MISS for key: {Key}", key);
                return Task.FromResult(default(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Cache] Error while getting key: {Key}", key);
                throw;
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };

                _cache.Set(key, value, options);
                _logger.LogDebug("[Cache] SET key: {Key} with expiration {Expiration}", key, expiration);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Cache] Error while setting key: {Key}", key);
                throw;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("[Cache] REMOVE key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Cache] Error while removing key: {Key}", key);
                throw;
            }
        }
    }
}
