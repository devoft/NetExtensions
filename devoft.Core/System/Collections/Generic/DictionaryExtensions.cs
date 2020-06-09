using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoft.System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueSelector)
            => (dict.TryGetValue(key, out var value))
                ? value
                : dict[key] = valueSelector();

        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : new()
            => dict.Ensure(key, () => new TValue());

        public static IDictionary<TKey, TValue> MergeWith<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other)
        {
            foreach (var item in other)
                dict[item.Key] = item.Value;
            return dict;
        }
    }

}
