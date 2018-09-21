using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System
{
    public class Disposable<T> : IDisposable
        where T : class
    {
        private EventHandler<T> _disposed;

        public Disposable(T value)
        {
            Value = value;
        }

        public Disposable(T value, EventHandler<T> onDisposed)
        {
            Value = value;
            Disposed += onDisposed;
        }

        public T Value { get; }
        public static Disposable<T> Empty { get; } = new Disposable<T>(default(T));

        public void Dispose()
        {
            var disposable = Value as IDisposable;
            disposable?.Dispose();
            OnDisposed(Value);
        }

        public event EventHandler<T> Disposed
        {
            add { ConcurrencyHelper.SafeChange(ref _disposed, x => x + value); }
            remove { ConcurrencyHelper.SafeChange(ref _disposed, x => x - value); }
        }

        private void OnDisposed(T args)
        {
            _disposed?.Invoke(this, args);
            Interlocked.Exchange(ref _disposed, (s, e) => { });
        }

        public static implicit operator T(Disposable<T> disposable)
            => disposable.Value;

        public static explicit operator Disposable<T>(T disposable)
            => disposable.AsDisposable();

    }
}
