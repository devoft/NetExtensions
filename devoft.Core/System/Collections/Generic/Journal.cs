using System;
using System.Collections.Generic;
using System.Text;

namespace devoft.Core.System.Collections.Generic
{
    public class Journal<T>
    {
        private Link _cursor, _firstDummy;
        private int _cursorIndex = -1;

        public Journal()
        {
            _firstDummy = new Link(default(T), null, null);
            _cursor = _firstDummy;
        }

        public void Push(T item)
        {
            _cursor = new Link(item, _cursor, null);
            _cursor.Previous.Next = _cursor;
            _cursorIndex++;
            Count = _cursorIndex + 1;
        }

        public T Peek()
            => IsOutOfrange 
                    ? throw new IndexOutOfRangeException()
                    : _cursor.Value;

        public bool IsOutOfrange 
            => _cursor == _firstDummy;

        public bool CanGoBack 
            => Count != 0 && _cursor != _firstDummy;

        public void GoBack()
        {
            if (!CanGoBack)
                throw new InvalidOperationException("Cannot go back");

            _cursor = _cursor.Previous;
            _cursorIndex--;
        }

        public bool CanGoForward 
            => Count != 0 && _cursor.Next != null;

        public void GoForward()
        {
            if (!CanGoForward)
                throw new InvalidOperationException("Cannot go forward");

            _cursor = _cursor.Next;
            _cursorIndex++;
        }

        public int Count { get; private set; }

        public class Link
        {
            public Link(T value, Link previous, Link next)
            {
                Previous = previous;
                Next = next;
                Value = value;
            }

            public Link Next { get; set; }
            public Link Previous { get; set; }
            public T Value { get; set; }
        }
    }
}
