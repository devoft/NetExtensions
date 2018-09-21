using System;
using System.Text;

namespace devoft.Core.Patterns.Scoping
{

    public class ScopeUnhandledException : Exception
    {
        public ScopeUnhandledException(string message)
            : base(message)
        {

        }
    }
}
