using StackExchange.Redis;

namespace ChatAppApi.Services
{
    public class RedisService
    {
        private readonly IDatabase _redis;

        public RedisService(IDatabase redis)
        {
            _redis = redis;
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _redis.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            return await _redis.StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _redis.KeyDeleteAsync(key);
        }
    }
}