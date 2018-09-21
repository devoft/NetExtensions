using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace devoft.Core.Patterns
{
    public interface IPropertyChangedNotifier : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
    }
}
