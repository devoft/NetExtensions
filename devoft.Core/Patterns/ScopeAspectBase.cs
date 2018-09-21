using devoft.Core.Patterns.Scoping;

namespace devoft.Core.Patterns
{
    public class ScopeAspectBase<TInheritor> : IScopeAspect
        where TInheritor : ScopeAspectBase<TInheritor>
    {
        public virtual bool Begin(ScopeContext context)
        {
            context.Add(GetType().Name, this);
            return true;
        }

        public virtual void End(ScopeContext context, bool result) { }

        public virtual bool IsInscope(ScopeContext context)
            => context.ContainsKey(GetType().Name);

        public virtual TInheritor ParentScope(ScopeContext context)
            => context.Parent?[GetType().Name] as TInheritor;
    }
}
