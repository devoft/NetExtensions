using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using devoft.Core.Patterns;
using devoft.Core.Patterns.Scoping;

namespace devoft.Core.Patterns
{
    /// <summary>
    /// Describe a scope aspect. Every time a scope is about to start, the <see cref="Begin(ScopeContext)"/>
    /// method of every registered scope aspect will be called in order. Once the scope ends, all the 
    /// <see cref="End(ScopeContext, bool)"/> methods will be called in reverse order
    /// </summary>
    public interface IScopeAspect
    {
        /// <summary>
        /// Executed when a Scope is about to start
        /// </summary>
        /// <param name="context">the context containing information useful to the aspect logic</param>
        /// <returns>whether the scope can continue or not</returns>
        bool Begin(ScopeContext context);
        /// <summary>
        /// Executed when the scope finishes
        /// </summary>
        /// <param name="context">the context containing information useful to the aspect logic</param>
        /// <param name="result">Indicates whether the scope succeded until its end</param>
        void End(ScopeContext context, bool result);
        /// <summary>
        /// Checks if this aspect is present on the active scope
        /// </summary>
        /// <param name="context">the context containing information useful to the aspect logic</param>
        /// <returns>true if the Aspect is enabled for the active scope, false otherwise</returns>
        bool IsInscope(ScopeContext context);
    }
}
