using devoft.System;
using devoft.System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace devoft.Core.Patterns.Scoping
{
    public interface IScopeTask
    {
        void SetResult(object result);
        ScopeContext CurrentScopeContext { get; }

    }

    public class ScopeTaskBase<TInheritor>
        : IDisposable,
          IScopeTask,
          IObservable<object>
        where TInheritor : ScopeTaskBase<TInheritor>, new()
    {
        protected internal HashSet<IObserver<object>> _observers = new HashSet<IObserver<object>>();
        protected List<IScopeAspect> _aspects = new List<IScopeAspect>();
        private Action<ScopeContext> _action;
        private Func<ScopeContext, Task> _asyncAction;
        private ScopeContext _parentContext;

        public ScopeTaskBase(IEnumerable<IScopeAspect> aspects)
        {
            if (aspects != null)
                _aspects.AddRange(aspects);
        }

        public ScopeTaskBase()
        {
        }

        public ScopeResult ResultStatus { get; private set; }
        public object LastResult { get; private set; }

        public static TInheritor Define(Action<ScopeContext> action)
        {
            var result = new TInheritor();
            result._action = action;
            return result;
        }

        public static TInheritor Define(Func<ScopeContext, Task> action)
        {
            var result = new TInheritor();
            result._asyncAction = action;
            return result;
        }


        public TInheritor AddAspects(params IScopeAspect[] aspects)
        {
            _aspects.AddRange(aspects);
            return this as TInheritor;
        }

        public TInheritor Observe(Action<IObservable<object>> subscribe)
        {
            subscribe?.Invoke(this);
            return this as TInheritor;
        }

        public TInheritor ScopedTo(ScopeContext parentContex)
        {
            _parentContext = parentContex;
            return this as TInheritor;
        }

        public Task<object> StartAsync(Action<ScopeContext> action)
        {
            _action = action;
            return StartAsync();
        }

        public async Task<object> StartAsync()
        {
            var context = new ScopeContext(this, _parentContext);
            CurrentScopeContext = context;
            var result = true;
            try
            {
                if (_asyncAction != null)
                {
                    if (result = OnBeginScope(context))
                        await _asyncAction(context);
                }
                else
                    await Task.Run(() =>
                    {
                        if (result = OnBeginScope(context))
                            _action(context);
                    });
                ResultStatus = ScopeResult.Success;
                return LastResult;
            }
            catch (OperationCanceledException)
            {
                ResultStatus = ScopeResult.Canceled;
            }
            catch (Exception ex)
            {
                _observers.ForEach(x => x.OnError(ex));
                ResultStatus = ScopeResult.Failed;
                throw;
            }
            finally
            {
                OnEndScope(context, result);
                Dispose();
            }
            return LastResult;
        }

        public ScopeContext CurrentScopeContext { get; private set; }

        protected virtual bool OnBeginScope(ScopeContext context)
        {
            var result = true;
            if (_aspects != null)
                foreach (var aspect in _aspects)
                    result &= aspect.Begin(context);
            return result;
        }

        protected virtual void OnEndScope(ScopeContext context, bool result)
            => _aspects.Reverse<IScopeAspect>().ForEach(aspect => aspect.End(context, result));

        public void Dispose()
        {
            _observers.OfType<IDisposable>().ForEach(x => x.Dispose());
            _observers = null;
        }

        public IDisposable Subscribe(IObserver<object> observer)
            => _observers.Add(observer)
                ? observer.AsDisposable((s, e) => _observers.Remove(e))
                : Disposable<IObserver<object>>.Empty;

        public void SetResult(object result)
        {
            LastResult = result;
            _observers.ForEach(x => x.OnNext(result));
        }
    }
}
