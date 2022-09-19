using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HT.Common.Caching
{
    public class InMemoryCache : ICaching
    {
        private static IMemoryCache __cache;
        static InMemoryCache()
        {            
            __cache = new MemoryCache(new MemoryCacheOptions()
            {
                CompactionPercentage = 0.2
            });
        }
        public void Delete(string key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public void Flush(int dbId)
        {
            throw new NotImplementedException();
        }

        public string Get(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key) where T : new()
        {
            return __cache.Get<T>(key);
        }

        public Task<string> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            return await Task.FromResult(__cache.Get<T>(key));
        }

        public Task<object[]> GetHashAllAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetHashAsync(string key, string fieldName)
        {
            throw new NotImplementedException();
        }

        public Task<object[]> GetHashAsync(string key, string[] fieldNames)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, int minutesToCache)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T value, int minutesToCache) where T : new()
        {
            __cache.Set(key, value, DateTimeOffset.Now.AddMinutes(minutesToCache));
        }

        public async Task<bool> SetAsync(string key, string value, int minutesToCache)
        {
            __cache.Set(key, value, DateTimeOffset.Now.AddMinutes(minutesToCache));
            return await Task.FromResult(true);
        }

        public Task<bool> SetAsync<T>(string key, T value, int minutesToCache) where T : new()
        {
            throw new NotImplementedException();
        }

        public void SetBatch(Dictionary<string, string> commands, int minutesToCache)
        {
            throw new NotImplementedException();
        }

        public void SetBatch<T>(Dictionary<string, T> commands, int minutesToCache) where T : new()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetHashAsync(string key, IDictionary<string, object> hash, int minutesToCache)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetHashAsync(string key, object value, string fieldName, int minutesToCache)
        {
            throw new NotImplementedException();
        }
    }
}
