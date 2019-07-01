using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoft.System.Threading.Tasks
{
    public static class TaskExtension
    {
        public static Task<TResult[]> SelectAsync<T, TResult>(this IEnumerable<T> collection, Func<T, Task<TResult>> asyncSelector)
            => Task.WhenAll(collection.Select(item => asyncSelector(item)));

        public static async Task<IEnumerable<TResult>> SelectManyAsync<T, TResult>(this IEnumerable<T> collection, Func<T, Task<IEnumerable<TResult>>> asyncSelector)
            => (await Task.WhenAll(collection.Select(asyncSelector))).SelectMany(s => s);
    }
}
