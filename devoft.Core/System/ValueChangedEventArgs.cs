using System;
using System.Collections.Generic;
using System.Text;

namespace devoft.Core.System
{
    public class ValueChangedEventArgs<T>
    {
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public T OldValue { get; }
        public T NewValue { get; }
    }
}
