using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace devoft.System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> items)
        {
            foreach (var item in items)
                col.Add(item);
        }

        public static T[] Remove<T>(this T[] array, T value)
        {
            Contract.Requires(array != null);

            return array.Except(new[] { value }).ToArray();
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            Contract.Requires(array != null);

            return array.Where((x, i) => i != index).ToArray();
        }

        public static void RemoveLast<T>(this IList<T> collection, int ammount = 1)
        {
            if (collection == null)
                return;
            var count = collection.Count;
            for (var i = 1; i <= ammount; i++)
                collection.RemoveAt(count - i);
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> col, T value) 
            => col.ToArray().Add(value);

        public static T[] Add<T>(this T[] array, T value)
        {
            var result = new T[array.Length + 1];
            Array.Copy(array, result, array.Length);
            result[array.Length] = value;
            return result;
        }
        
        public static bool IsSorted<T>(
            this IEnumerable<T> collection, 
            IComparer<T> comparer, 
            bool resultIfEmpty = false, 
            bool descending = false)
        {
            if (collection == null)
                return resultIfEmpty;

            var comparables = collection as T[] ?? collection.ToArray();
            if (!comparables.Any())
                return resultIfEmpty;

            var old = default(T);
            var first = true;
            var descendingFactor = descending ? 1 : -1;
            foreach (var current in comparables)
            {
                if (!first && (comparer.Compare(old, current) * descendingFactor) < 0)
                    return false;
                old = current;
                first = false;
            }
            return true;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
            => collection == null
                     ? new HashSet<T>()
                     : (collection as HashSet<T>) ?? new HashSet<T>(collection);

        public static Dictionary<TKey, TValue> SafeToDictionary<T, TKey, TValue>(this IEnumerable<T> collection, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
            => collection?.ToDictionary(keySelector, valueSelector) ?? new Dictionary<TKey, TValue>();

        public static T[] Insert<T>(this T[] array, int index, T value)
        {
            var result = new T[array.Length + 1];
            Array.Copy(array, index, result, index + 1, array.Length - index);
            result[index] = value;
            return result;
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T elem)
        {
            var list = enumerable as IList;
            if (list != null)
                return list.IndexOf(elem);
            var listSource = enumerable as IListSource;
            if (listSource != null)
                return listSource.GetList().IndexOf(elem);
            var index = 0;
            foreach (var item in enumerable)
            {
                if (Equals(item, elem))
                    return index;
                index++;
            }
            return -1;
        }

        public static bool AreEqual<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return false;
            var value = default(T);
            var started = false;
            foreach (var item in collection)
                if (!started)
                {
                    value = item;
                    started = true;
                }
                else if (!Equals(item, value))
                    return false;
            return started;
        }

        public static bool CollectionEquals<T>(this IEnumerable<T> collection, IEnumerable<T> other)
        {
            if (Equals(collection, other))
                return true;
            if (collection == null || other == null)
                return false;

            var col1 = collection.ToArray();
            var col2 = other.ToArray();

            return col1.Length == col2.Length && !col1.Where((t, i) => !Equals(t, col2[i])).Any();
        }

        public static void Update<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            collection.Clear();
            collection.AddRange(other);
        }
    }
}
