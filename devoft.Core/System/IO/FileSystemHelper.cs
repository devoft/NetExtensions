using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    public static class FileSystemHelper
    {
        public static string EnsureDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
