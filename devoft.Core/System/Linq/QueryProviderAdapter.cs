using System.Linq;
using System.Linq.Expressions;

namespace devoft.System.Linq
{
    /// <summary>
    /// Base class of QueryProvider Adapters. This providers works with <see cref="QueryableAdapter{TElement}"/>
    /// </summary>
    /// <typeparam name="TQueryable"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class QueryProviderAdapter<TQueryable, TElement> : IQueryProvider
        where TQueryable : QueryableAdapter<TElement>
    {
        protected IQueryProvider _wrapped;

        protected internal QueryProviderAdapter(IQueryProvider wrapped)
        {
            _wrapped = wrapped;
        }

        public virtual IQueryable<TItem> CreateQuery<TItem>(Expression expression)
            => new QueryableAdapter<TItem>(_wrapped.CreateQuery<TItem>(expression));

        public IQueryable CreateQuery(Expression expression)
            => CreateQuery<TElement>(expression);

        public virtual TResult Execute<TResult>(Expression expression)
            => (TResult)_wrapped.CreateQuery<TElement>(expression);

        public object Execute(Expression expression)
            => Execute<object>(expression);
    }


}
