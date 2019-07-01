using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using devoft.Core.Patterns;
using devoft.System;

namespace devoft.System.Collections.Generic
{
    public class ScopedDictionary<TKey, TValue>
        : HierarchicalObject<ScopedDictionary<TKey, TValue>>,
          IDictionary<TKey, TValue>, IHierarchicalObject<ScopedDictionary<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();
        private ScopedDictionary<TKey, TValue> _parent;

        public ScopedDictionary(ScopedDictionary<TKey, TValue> parent = null)
        {
            Parent = parent;
        }

        public TValue this[TKey key]
        {
            get => _items.TryGetValue(key, out var result)
                        ? result
                        : Parent != null
                            ? Parent[key]
                            : throw new ArgumentException("Key was not present in the dictionary", nameof(key));
            set => _items[key] = value;
        }

        public ICollection<TKey> Keys
            => Parent == null
                ? _items.Keys
                : (ICollection<TKey>)_items.Keys.Union(Parent.Keys).ToList();

        public ICollection<TValue> Values
            => Parent == null
                ? _items.Values
                : (ICollection<TValue>)_items.Values.Union(Parent.Values).ToList();

        public int Count => Keys.Count();

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
            => _items.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item)
            => _items.Add(item.Key, item.Value);

        public void Clear()
            => _items.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => _items.Contains(item) || Parent?.Contains(item) == true;

        public bool ContainsKey(TKey key)
            => _items.ContainsKey(key) || Parent?.ContainsKey(key) == true;

        public bool LocalContainsKey(TKey key)
            => _items.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new IndexOutOfRangeException();

            var index = arrayIndex;
            foreach (var x in this)
            {
                if (array.Length == index)
                    return;
                array[index] = x;
                index++;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => Parent == null
                ? _items.GetEnumerator()
                : _items.Union(Parent).GetEnumerator();

        public bool Remove(TKey key)
            => _items.Remove(key) && Parent?.ContainsKey(key) != true;

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);

        public bool TryGetValue(TKey key, out TValue value)
            => _items.TryGetValue(key, out value) || Parent?.TryGetValue(key, out value) == true;

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
