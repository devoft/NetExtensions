using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Recorre la <b>collection</b> aplicando la acción indicada en <b>action</b> recibiendo como 
        /// parámetro cada valor de la iteración
        /// </summary>
        /// <typeparam name="T">Tipo genérico de la colección</typeparam>
        /// <param name="collection">colección de objetos</param>
        /// <param name="action">acción a realizar por cada iteración</param>
        /// <returns>Array resultante de la iteración</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
            => LateForEach(collection, action).ToArray();

        /// <summary>
        /// Recorre la <b>collection</b> aplicando la acción indicada en <b>action</b> recibiendo como 
        /// parámetro cada valor de la iteración
        /// </summary>
        /// <typeparam name="T">Tipo genérico de la colección</typeparam>
        /// <param name="collection">colección de objetos</param>
        /// <param name="action">acción a realizar por cada iteración</param>
        /// <returns>Enumerable resultante de la iteración</returns>
        /// <remarks>
        /// </remarks>
        public static IEnumerable<T> LateForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                yield break;
            foreach (var elem in collection)
            {
                action(elem);
                yield return elem;
            }
        }


        public static IEnumerable<T> ForEachParallel<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var res = Parallel.ForEach(collection, x => action(x));
            return collection;
        }

        public static bool None<T>(this IEnumerable<T> collection)
            => !collection.Any();

        public static IEnumerable<T> OfType<T>(this IEnumerable<T> source, Type targetType)
            => source.Where(item => item.GetType().IsSubclassOf(targetType));

        public static T[] SafeToArray<T>(this IEnumerable<T> col)
            => col?.ToArray() ?? Array.Empty<T>();

        public static IEnumerable<T> OrElse<T>(this IEnumerable<T> result, IEnumerable<T> def)
        {
            var array = result.SafeToArray();
            return array.Any() ? def : array;
        }

        public static IEnumerable<T> DisposeAll<T>(this IEnumerable<T> collection)
            where T : IDisposable
        {
            var disposeAll = collection as T[] ?? collection.ToArray();
            foreach (var item in disposeAll)
                item.Dispose();
            return disposeAll;
        }

        public static bool In<T>(this T x, params T[] array) => array?.Contains(x) == true;


        public static string ToString(this IEnumerable collection, string separator)
            => string.Join(separator, collection.Cast<object>());
    }
}
