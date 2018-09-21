using System;
using devoft.Core.System.Collections.Generic;

namespace devoft.Core.Patterns.Scoping
{
    public class ScopeContext : ScopedDictionary<string, object>
    {
        private IScopeTask _owner;

        internal ScopeContext(IScopeTask owner, ScopeContext parentContext)
        { 
            _owner = owner;
            Parent = parentContext;
        }

        public ScopeContext()
        {
        }

        public void Yield(object result)
            => _owner?.SetResult(result);

        public void Cancel()
            => throw new OperationCanceledException();

        public void Set<T>(T value)
            => this[typeof(T).Name] = value;

        public void Unset<T>()
            => Remove(typeof(T).Name);

        public T Get<T>()
            => ContainsKey<T>()
                ? (T)this[typeof(T).Name]
                : default(T);

        public bool ContainsKey<T>()
            => ContainsKey(typeof(T).Name);

        public T GetRequired<T>()
            => ContainsKey<T>()
                ? (T)this[typeof(T).Name]
                : throw new InvalidOperationException($"Key {typeof(T).Name} is required");
    }

}
