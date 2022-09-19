using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HT.Common.Caching
{
    public interface ICaching : IDisposable
    {
        void Delete(string key);
        void Set(string key, string value, int minutesToCache);

        void SetBatch(Dictionary<string, string> commands, int minutesToCache);
        void Set<T>(string key, T value, int minutesToCache) where T : new();

        void SetBatch<T>(Dictionary<string, T> commands, int minutesToCache) where T : new();
        bool Exists(string key);
        string Get(string key);
        T Get<T>(string key) where T : new();

        Task<string> GetAsync(string key);
        Task<T> GetAsync<T>(string key) where T : class;

        void Flush(int dbId);

        Task<bool> SetAsync(string key, string value, int minutesToCache);

        Task<bool> SetAsync<T>(string key, T value, int minutesToCache) where T : new();

        Task<bool> SetHashAsync(string key, IDictionary<string, object> hash, int minutesToCache);


        Task<object> GetHashAsync(string key, string fieldName);


        Task<object[]> GetHashAsync(string key, string[] fieldNames);
        Task<object[]> GetHashAllAsync(string key);
        Task<bool> SetHashAsync(string key, object value, string fieldName, int minutesToCache);
    }
 
}