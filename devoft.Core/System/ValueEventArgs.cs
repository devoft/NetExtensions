using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public class ValueEventArgs<TValue> : EventArgs
    {
        public ValueEventArgs(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; private set; }
    }
}
