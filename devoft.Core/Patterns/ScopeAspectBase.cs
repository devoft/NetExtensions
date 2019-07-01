using devoft.Core.Patterns.Scoping;

namespace devoft.Core.Patterns
{
    /// <summary>
    /// Default implementation of <see cref="IScopeAspect"/>
    /// </summary>
    /// <typeparam name="TInheritor">The Inheritor type</typeparam>
    public class ScopeAspectBase<TInheritor> : IScopeAspect
        where TInheritor : ScopeAspectBase<TInheritor>
    {
        /// <summary>
        /// Adds this aspect on context by type name
        /// </summary>
        /// <param name="context"><see cref="ScopeContext"/> dictionary </param>
        public virtual bool Begin(ScopeContext context)
        {
            context.Add(GetType().Name, this);
            return true;
        }

        /// <summary>
        /// Remove this aspect from context
        /// </summary>
        /// <param name="context"><see cref="ScopeContext"/> dictionary </param>
        public virtual void End(ScopeContext context, bool result)
        {
            context.Remove(GetType().Name);
        }

        /// <summary>
        /// Returns true if the aspect name is on context
        /// </summary>
        /// <param name="context"><see cref="ScopeContext"/> dictionary </param>
        public virtual bool IsInscope(ScopeContext context)
            => context.ContainsKey(GetType().Name);

        /// <summary>
        /// Returns the parent scope aspect instance of this scope aspect 
        /// </summary>
        /// <param name="context"><see cref="ScopeContext"/> dictionary</param>
        public virtual TInheritor ParentScope(ScopeContext context)
            => context.Parent?[GetType().Name] as TInheritor;
    }
}
