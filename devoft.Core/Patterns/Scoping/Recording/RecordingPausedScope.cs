using System;

namespace devoft.Core.Patterns.Scoping
{
    public class RecordingPausedScope : IDisposable
    {
        private ScopeContext _context;
        private ScopeRecorder _recorder;

        public RecordingPausedScope(ScopeContext context, ScopeRecorder recorder)
        {
            _context = context;
            _recorder = recorder;
        }

        public void Dispose()
        {
            _recorder.ResumeRecording(_context);
        }
    }
}
