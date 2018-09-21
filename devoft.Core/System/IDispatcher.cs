using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public interface IDispatcher
    {
        Task InvokeAsync(Action action);
    }
}
