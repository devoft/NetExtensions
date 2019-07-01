using devoft.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace devoft.Core.Patterns
{
    public class Cache<TMemo, TKey, TValue>
            where TMemo : class, ICacheMemo<TKey, TValue>, new()
    {
        private readonly SortedSet<TMemo> _set;
        private readonly Dictionary<TKey, TMemo> _dictionary = new Dictionary<TKey, TMemo>();
        private readonly int _capacity;

        public Cache(Comparison<TMemo> comparison, int capacity)
        {
            _set = new SortedSet<TMemo>(comparison.ToComparer());
            _capacity = capacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (Equals(key, default(TKey)))
                throw new ArgumentNullException(nameof(key));
            if (Equals(value, default(TValue)))
                throw new ArgumentNullException(nameof(value));
            var memo = new TMemo { Key = key, Value = value };
            var memoToRemove = default(TMemo);
            if (_set.Count == _capacity)
                memoToRemove = _set.Min;

            _set.Add(memo);
            _dictionary.Add(key, memo);

            if (memoToRemove == null)
                return;

            _dictionary.Remove(memoToRemove.Key);
            _set.Remove(memoToRemove);
        }

        public void Remove(TKey key)
        {
            var memo = _dictionary[key];
            _set.Remove(memo);
            _dictionary.Remove(key);
        }

        public TValue this[TKey item]
        {
            get
            {
                if (!_dictionary.ContainsKey(item))
                    return default(TValue);
                var memo = _dictionary[item];
                _set.Remove(memo);
                memo.Update();
                _set.Add(memo);
                return memo.Value;
            }
        }

        public bool Contains(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
            _set.Clear();
        }
    }

    public interface ICacheMemo<TKey, TValue> : IEquatable<ICacheMemo<TKey, TValue>>
    {
        TKey Key { get; set; }
        TValue Value { get; set; }
        void Update();
    }

    public class TicksMemo<TKey, TValue> : ICacheMemo<TKey, TValue>
    {
        public TicksMemo()
        {
            Ticks = DateTime.Now.Ticks;
        }

        public long Ticks { get; set; }

        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(Key, ((TicksMemo<TKey, TValue>)obj).Key);
        }

        bool IEquatable<ICacheMemo<TKey, TValue>>.Equals(ICacheMemo<TKey, TValue> other)
        {
            return Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static Comparison<TicksMemo<TKey, TValue>> DesiredComparison
        {
            get { return (x, y) => Math.Sign(x.Ticks - y.Ticks); }
        }

        public void Update()
        {
            Ticks = DateTime.Now.Ticks;
        }
    }
}
