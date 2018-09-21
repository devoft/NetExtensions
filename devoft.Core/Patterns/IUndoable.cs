using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace devoft.Core.Patterns
{
    public interface IUndoable
    {
        Task Undo();
        Task Redo();
    }
}
