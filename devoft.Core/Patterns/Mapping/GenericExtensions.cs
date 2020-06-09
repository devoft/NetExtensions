using devoft.System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace devoft.Core.Patterns.Mapping
{
    public static class GenericExtensions
    {
        public static TResult Map<TSource, TResult>(this TSource source, object mapping = null, bool includeBaseType = false)
            where TResult : new()
            => MapTo(source, new TResult(), mapping, includeBaseType);

        public static TResult MapTo<TSource, TResult>(this TSource source, TResult target, object mapping = null, bool includeBaseType = false)
        {
            var mapped = mapping?.ToDictionary() ?? new Dictionary<string, object>();
            return target.FromDictionary(source.ToDictionary()
                                               .Where(x => !mapped.ContainsKey(x.Key))
                                               .ToDictionary(x => x.Key, x => x.Value)
                                               .MergeWith(mapped), 
                                         includeBaseType);
        }

        public static T Merge<T>(this T source, object addings, bool includeBaseType = false)
            => source.FromDictionary(addings.ToDictionary(includeBaseType), includeBaseType);

        public static IDictionary<string, object> ToDictionary(this object obj, bool includeBaseType = false)
        {
            var result = new Dictionary<string, object>();
            if (obj != null)
            {
                var flags = BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.GetProperty
                            | BindingFlags.SetProperty;
                if (!includeBaseType)
                    flags |= BindingFlags.DeclaredOnly;

                var props = obj.GetType()
                               .GetProperties(flags);
                foreach (var prop in props)
                    result[prop.Name] = prop.GetValue(obj);
            }
            return result;
        }

        public static T FromDictionary<T>(this T obj, IDictionary<string, object> dict, bool includeBaseType = false)
        {
            if (dict?.Any() == true)
            {
                var flags = BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.GetProperty
                            | BindingFlags.SetProperty;
                if (!includeBaseType)
                    flags |= BindingFlags.DeclaredOnly;
                var allprops = obj.GetType()
                                  .GetProperties(flags)
                                  .ToDictionary(x => x.Name, x => x);
                foreach (var kv in dict.Where(x => allprops.ContainsKey(x.Key)))
                    allprops[kv.Key].SetValue(obj, kv.Value);
            }
            return obj;
        }
    }
}
