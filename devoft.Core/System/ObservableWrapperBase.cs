using System;

namespace devoft.System
{
    public abstract class ObservableWrapperBase<T> : IObservable<T>
    {
        protected abstract IObservable<T> SurrogateObservable { get; }
        public IDisposable Subscribe(IObserver<T> observer)
            => SurrogateObservable.Subscribe(observer);
    }
}
