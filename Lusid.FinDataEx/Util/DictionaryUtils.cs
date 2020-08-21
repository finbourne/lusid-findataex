using System.Collections.Generic;

namespace Lusid.FinDataEx.Util
{
    public static class DictionaryUtils
    {
        public static TValue GetOrCreateAndPut<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) 
            where TValue : new()
        {
            TValue val;

            if (!dict.TryGetValue(key, out val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }
    }
}