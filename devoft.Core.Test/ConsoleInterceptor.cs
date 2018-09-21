using System.IO;
using System.Text;
using System.Collections.Generic;

namespace devoft.Core.Test
{
    public class ConsoleInterceptor : TextWriter
    {
        public string LastText { get; private set; }
        public List<string> Lines { get; } = new List<string>();

        public override Encoding Encoding => Encoding.Default;

        public override void WriteLine(string value)
        {
            LastText = value;
            Lines.Add(value);
        }

        public void Reset()
        {
            Lines.Clear();
            LastText = null;
        }
    }
}
