using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace devoft.System
{
    public class ConcurrencyHelper
    {
        public static void SafeChange<T>(ref T operand, Func<T, T> change)
            where T : class
        {
            var oldValue = operand;
            while (true)
            {
                var currentValue = oldValue;
                var nextValue = change(currentValue);
                oldValue = Interlocked.CompareExchange(ref operand, nextValue, currentValue);
                if (Equals(oldValue, currentValue))
                    break;
            }
        }

        public static async void SubscribeWhileNull(INotifyPropertyChanged source, string propertyName, Action action)
        {
            var propInfo = source.GetType().GetProperty(propertyName);
            var value = propInfo.GetValue(source);
            var tcs = new TaskCompletionSource<bool>();
            if (value == null)
            {
                 PropertyChangedEventHandler handle = (s, e) =>
                                              {
                                                  if (e.PropertyName != propertyName) 
                                                      return;
                                                  value = propInfo.GetValue(source);
                                                  if (value == null)
                                                      return;

                                                  action();
                                                  tcs.SetResult(true);
                                              };
                source.PropertyChanged += handle;
                await tcs.Task;
                source.PropertyChanged -= handle;
            }
            else action();
        }
    }
}
