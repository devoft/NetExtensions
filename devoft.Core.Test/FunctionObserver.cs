using System;

namespace devoft.Core.Test
{
    public class FunctionObserver<T> : IObserver<T>
    {
        private Action<T> _action;
        private Action<Exception> _error;

        public FunctionObserver(Action<T> action = null, Action<Exception> error = null)
        {
            _action = action;
            _error = error;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _error?.Invoke(error);
        }

        public void OnNext(T value)
        {
            _action?.Invoke(value);
        }
    }
}