using System;
using System.Collections.Generic;
using System.Text;

namespace devoft.Core.Patterns
{
    public static class Singleton<T>
        where T : class
    {
        public static T Instance { get; private set; }

        public static T Create(Func<T> ctor)
            => Instance != null
                ? Instance
                : Instance = ctor();
    }

}
