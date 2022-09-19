using System;
using System.Collections.Generic;
using System.Text;

namespace HT.Common.Caching
{
    public interface IConcurrentCaching : ICaching
    {
        bool SetConcurrent<T>(string key, T originalValue, T newValue) where T : new();
        int IncrementKey(string key, int increment = 1);
        IEnumerable<T> GetCacheList<T>(string key);
        bool RemoveAllListMember<T>(string key, T value);
        bool AddListMember<T>(string key, T value, int? minutesToExpire);
        bool AddListMembers<T>(string key, IEnumerable<T> value, int? minutesToExpire);
    }
}
