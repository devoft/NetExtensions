using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace devoft.System.Collections.Specialized
{
    public abstract class ObservableViewBase<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        protected T[] ViewArray = new T[0];
        private NotifyCollectionChangedEventHandler _collectionChangedHandler;

        public IEnumerator<T> GetEnumerator() 
            => ViewArray.Cast<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public virtual T this[int index] 
            => ViewArray[index];

        public async Task UpdateAsync(Task<IEnumerable<T>> collectionBuildTask, CancellationToken token)
        {
            var collection = await collectionBuildTask;
            await UpdateAsync(collection, token);
        }

        public void Update(params T[] others)
            => Update((IEnumerable<T>)others);

        public void Update(IEnumerable<T> others)
        {
            if (others == null)
                return;
            lock (this)
            {
                var mergeRecords = UpdateOverride(others);
                foreach (var args in mergeRecords
                    .Select(record => 
                        record.IsMove
                        ? new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Move,
                            record.Items[0], record.NewIndex, record.StartIndex)
                        : record.IsReplace
                            ? new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                record.Items, record.OldItems,
                                record.StartIndex)
                            : new NotifyCollectionChangedEventArgs(
                                record.IsAdd
                                    ? NotifyCollectionChangedAction.Add
                                    : NotifyCollectionChangedAction.Remove,
                                record.Items,
                                record.StartIndex)))
                    OnCollectionChanged(args);
            }
        }

        public Task UpdateAsync(IEnumerable<T> collection) 
            => UpdateAsync(collection, new CancellationToken(false));

        public async Task UpdateAsync(IEnumerable<T> others, CancellationToken token)
        {
            IEnumerable<MergeRecord> mergeRecords;
            lock (this)
                mergeRecords = UpdateOverride(others);
            token.ThrowIfCancellationRequested();
            await Task.Yield();
            Action notifyChanges = () =>
            {
                foreach (var args in mergeRecords.Select(record => record.IsMove
                                                                 ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, record.Items[0], record.NewIndex, record.StartIndex)
                                                                 : record.IsReplace 
                                                                    ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, record.Items, record.OldItems, record.StartIndex)
                                                                    : new NotifyCollectionChangedEventArgs(record.IsAdd
                                                                                                             ? NotifyCollectionChangedAction.Add
                                                                                                             : NotifyCollectionChangedAction.Remove,record.Items,record.StartIndex)))
                    OnCollectionChanged(args);
            };

            notifyChanges();
        }

        public void Clear() 
            => Update(Enumerable.Empty<T>());

        public int Count 
            => ViewArray.Length;

        public bool HasItems => Count > 0;

        public IList<T> Local => ViewArray;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { ConcurrencyHelper.SafeChange(ref _collectionChangedHandler, x => x + value);}
            remove { ConcurrencyHelper.SafeChange(ref _collectionChangedHandler, x => x - value); }
        }

        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _collectionChangedHandler?.Invoke(this, e);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(HasItems));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        protected abstract IEnumerable<MergeRecord> UpdateOverride(IEnumerable<T> others);

        protected class MergeRecord
        {
            public int StartIndex;
            public int NewIndex;
            public bool IsAdd;
            public bool IsReplace;
            public bool IsMove;
            public IList Items = new List<T>();
            public readonly IList OldItems = new List<T>();

            public MergeRecord()
            {
            }

            public void Update(object item, bool? isAdd = null, int? index = null)
            {
                Items.Add(item);
                if (isAdd.HasValue)
                    IsAdd = isAdd.Value;
                if (index.HasValue)
                    StartIndex = index.Value;
            }

            public void UpdateReplace(object oldItem, object item, int? index = null)
            {
                OldItems.Add(oldItem);
                IsReplace = true;
                Items.Add(item);
                if (index.HasValue)
                    StartIndex = index.Value;
            }

            public static MergeRecord ForMove(object item, int oldIndex, int newIndex)
            {
                var result = new MergeRecord();
                result.Items.Add(item);
                result.IsMove = true;
                result.StartIndex = oldIndex;
                result.NewIndex = newIndex;
                return result;
            }
        }
    }
}