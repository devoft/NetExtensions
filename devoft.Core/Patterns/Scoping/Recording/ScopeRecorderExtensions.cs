using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devoft.Core.Patterns.Scoping
{
    public static class ScopeRecorderExtensions
    {
        public static TScopeTask Recording<TScopeTask>(this TScopeTask scopeTask)
            where TScopeTask : ScopeTaskBase<TScopeTask>, new()
            => scopeTask.AddAspects(new ScopeRecorder());

        public static RecordingPausedScope PauseRecording(this ScopeContext context)
            => context.TryGetValue(nameof(ScopeRecorder), out var recorder)
                    ? (recorder as ScopeRecorder)?.PauseRecording(context)
                    : null;

        public static async Task UndoAsync<TScopeTask>(this TScopeTask context)
            where TScopeTask : IScopeTask
        {
            if (context.CurrentScopeContext.Parent != null)
                throw new InvalidOperationException("Cannot Undo sub scopes");
            if (context.CurrentScopeContext.TryGetValue(nameof(ScopeRecorder), out var recorder) && 
                recorder is ScopeRecorder scopeRecorder)
                using (context.CurrentScopeContext.PauseRecording())
                    foreach (var x in scopeRecorder.Records
                                                   .OfType<IUndoable>()
                                                   .Reverse()
                                                   .ToArray())
                        await x.Undo();
        }

        public static async Task RedoAsync<TScopeTask>(this TScopeTask context)
            where TScopeTask : IScopeTask
        {
            if (context.CurrentScopeContext.Parent != null)
                throw new InvalidOperationException("Cannot Redo sub scopes");
            if (context.CurrentScopeContext.TryGetValue(nameof(ScopeRecorder), out var recorder) &&
                recorder is ScopeRecorder scopeRecorder)
                using (context.CurrentScopeContext.PauseRecording())
                    foreach (var x in scopeRecorder.Records
                                                   .OfType<IUndoable>()
                                                   .ToArray())
                        await x.Redo();
        }
    }
}
