using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace devoft.System.Linq
{
    /// <summary>
    /// Base class for Queryable adapters. This queryables combines with <see cref="QueryProviderAdapter{TQueryable, TElement}"/>
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class QueryableAdapter<TElement> : IOrderedQueryable<TElement>
    {
        protected IQueryable<TElement> _query;

        public QueryableAdapter(IQueryable<TElement> query)
        {
            _query = query;
            Provider = BuildQueryProviderOverride(_query.Provider);
        }

        public QueryableAdapter(IQueryable query) : this((IQueryable<TElement>)query) { }

        public Type ElementType
            => typeof(TElement);

        public virtual Expression Expression
            => _query.Expression;

        public virtual IQueryProvider Provider { get; }

        public IEnumerator<TElement> GetEnumerator()
            => Provider.Execute<IQueryable<TElement>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        protected virtual IQueryProvider BuildQueryProviderOverride(IQueryProvider wrapped)
            => new QueryProviderAdapter<QueryableAdapter<TElement>, TElement>(wrapped);
    }


}
