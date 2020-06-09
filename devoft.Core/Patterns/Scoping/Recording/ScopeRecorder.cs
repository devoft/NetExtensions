using System.Collections.Generic;
using System.Text;

namespace devoft.Core.Patterns.Scoping
{
    public class ScopeRecorder : ScopeAspectBase<ScopeRecorder>
    {
        private List<object> _records = new List<object>();

        public void Record(ScopeContext context, object record)
        {
            if (IsRecording(context))
                _records.Add(record);
        }

        public RecordingPausedScope PauseRecording(ScopeContext context)
        {
            var result = new RecordingPausedScope(context, this);
            context.Set(result);
            return result;
        }

        public override void End(ScopeContext context, bool result)
        {
        }

        public IEnumerable<object> Records
            => _records;

        internal void ResumeRecording(ScopeContext context)
            => context.Remove(nameof(RecordingPausedScope));

        public bool IsRecording(ScopeContext context)
            => !context.ContainsKey(nameof(RecordingPausedScope));
    }
}
