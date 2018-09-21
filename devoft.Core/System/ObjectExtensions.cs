using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devoft.Core.Patterns;

namespace System
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Asegura la devolución de un objeto <see cref="Disposable{T}"/> a partir de cualquier objeto.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a tratar como <see cref="Disposable{T}"/></typeparam>
        /// <param name="value">valor a envolver en un objeto <see cref="Disposable{T}"/></param>
        /// <param name="onDisposed">acción a realizar cuando el <see cref="Disposable{T}"/> sea liberado</param>
        /// <returns><b>null</b> si <b>value</b> es <b>null</b>, el propio valor de <b>value</b> si ya fuera <see cref="Disposable{T}"/>, 
        /// y en otro caso: un nuevo objeto de tipo <see cref="Disposable{T}"/> envolviendo el valor de <b>value</b></returns>
        public static Disposable<T> AsDisposable<T>(this T value, EventHandler<T> onDisposed = null)
            where T : class
        {
            if (value == null)
                return null;

            var alreadyDisposable = value as Disposable<T>;
            if (alreadyDisposable != null)
                return alreadyDisposable;

            var result = new Disposable<T>(value);
            if (onDisposed != null)
                result.Disposed += onDisposed;
            return result;
        }

        public static T OrElse<T>(this T result, T def)
            => Equals(result, default(T)) ? def : result;

        public static T OrElse<T>(this T result, Func<T> def)
            => Equals(result, default(T)) ? def() : result;

        public static TResult Safe<T, TResult>(this T item, Func<T, TResult> selector)
            => Equals(item, default(T)) ? default(TResult) : selector(item);


        public static TResult Cache<TParam1, TParam2, TResult>(this TParam1 arg1, TParam2 arg2, Func<TParam1, TParam2, TResult> selector, int cacheLength = 128)
            where TResult : class
        {
            var tuple = Tuple.Create(arg1, arg2);
            return Cache(tuple, x => selector(x.Item1, x.Item2), cacheLength);
        }

        public static TResult Cache<TParam, TResult>(this TParam arg, Func<TParam, TResult> selector, int cacheLength = 128)
            where TResult : class
        {
            var cache = Singleton<Cache<TicksMemo<TParam, TResult>, TParam, TResult>>.Create(() => new Cache<TicksMemo<TParam, TResult>, TParam, TResult>(TicksMemo<TParam, TResult>.DesiredComparison, cacheLength));

            var result = cache[arg];
            if (Equals(result, default(TResult)))
            {
                result = selector(arg);
                if (result != null)
                    cache.Add(arg, result);
            }

            return result;
        }

        public static Task<TResult> CacheAsync<TParam1, TParam2, TResult>(this TParam1 arg1, TParam2 arg2, Func<TParam1, TParam2, TResult> selector, int cacheLength = 128)
                    where TResult : class
        {
            var tuple = Tuple.Create(arg1, arg2);
            return CacheAsync(tuple, x => selector(x.Item1, x.Item2), cacheLength);
        }

        public static async Task<TResult> CacheAsync<TParam, TResult>(this TParam arg, Func<TParam, TResult> selector, int cacheLength = 128)
            where TResult : class
        {
            var cache = Singleton<Cache<TicksMemo<TParam, TResult>, TParam, TResult>>.Create(() => new Cache<TicksMemo<TParam, TResult>, TParam, TResult>(TicksMemo<TParam, TResult>.DesiredComparison, cacheLength));

            var result = cache[arg];
            if (Equals(result, default(TResult)))
                result = await Task.Factory.StartNew(() =>
                {
                    var res = selector(arg);
                    if (result != null)
                        cache.Add(arg, res);
                    return res;
                });

            return result;
        }

        public static void RemoveFromCache<TParam, TResult>(this TParam arg)
        {
            var cache = Singleton<Cache<TicksMemo<TParam, TResult>, TParam, TResult>>.Instance;
            if (cache == null)
                return;

            var result = cache[arg];
            if (!Equals(result, default(TResult)))
                cache.Remove(arg);
        }

        /// <summary>
        /// Apply a condition on a single element, if true, apply the 
        /// <paramref name="next"/> function to this object to get another 
        /// element until the condition fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="condition"></param>
        /// <param name="next"></param>
        /// <returns>Last element fulfilling the condition</returns>
        public static T AfterWhile<T>(this T t, Func<T, bool> condition, Func<T, T> next)
        {
            while (condition(t))
                t = next(t);
            return t;
        }

        public static T SafeAs<T>(this object obj) => (T)(obj ?? default(T));

        public static void Set(this object target, string propertyName, object value)
            => target.GetType().GetProperty(propertyName).SetValue(target, value);
        public static T Get<T>(this object target, string propertyName)
            => (T) target.GetType().GetProperty(propertyName).GetValue(null);


        public static bool IsInstanceOf<T>(this object obj)
        {
            return IsInstanceOf(obj, typeof(T));
        }

        public static bool IsInstanceOf(this object obj, Type typeT)
        {
            if (obj == null)
                return false;
            var type = obj.GetType();
            return typeT.IsAssignableFrom(type) ||
                typeT.IsGenericTypeDefinition && type.GetBaseTypes().Any(t => t.GetGenericTypeDefinition() == typeT);
        }
    }
}
