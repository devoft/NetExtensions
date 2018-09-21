using System;
using System.Collections.Generic;
using System.Text;

namespace devoft.Core.System
{
    public class Handler<T>
    {
        public Handler(T value = default(T))
        {
            Value = value;
        }

        public T Value { get; set; }

        public static explicit operator T(Handler<T> handler) => handler.Value;
        public static implicit operator Handler<T>(T val) => new Handler<T>(val);

    }
}
