using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using devoft.Core.Patterns;
using devoft.Core.Patterns.Scoping;

namespace devoft.Core.Patterns
{

    public interface IScopeAspect
    {
        bool Begin(ScopeContext context);
        void End(ScopeContext context, bool result);
        bool IsInscope(ScopeContext context);
    }
}
