using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Specialized
{
    public class ObservableSortedView<T> : ObservableViewBase<T>, ICollection<T>
    {
        public ObservableSortedView(Comparison<T> comparison)
        {
            Comparison = comparison;
        }

        public ObservableSortedView(IComparer<T> comparer)
            : this(comparer.Compare)
        {
        }

        public Comparison<T> Comparison { get; private set; }

        public void Add(T item)
        {
            var list = this.ToList();
            list.Add(item);
            list.Sort(Comparison);
            Update(list);
        }

        public bool Contains(T item)
        {
            return Array.BinarySearch(ViewArray, item, Comparison.ToComparer()) > 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ViewArray.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var list = this.ToList();
            var result = list.Remove(item);
            if (result)
                Update(list);
            return result;
        }

        public void Refresh()
        {
            var array = (T[])ViewArray.Clone();
            Array.Sort(array, Comparison.ToComparer());
            Update(array);
        }

        public bool IsReadOnly => false;

        protected override IEnumerable<MergeRecord> UpdateOverride(IEnumerable<T> others)
        {
            if (others == null)
            {
                Clear();
                return Enumerable.Empty<MergeRecord>();
            }

            var myEnumerator = GetEnumerator();
            var othersArray = others as T[] ?? others.ToArray();
            var otherEnumerator = othersArray.GetEnumerator();
            var thisHas = myEnumerator.MoveNext();
            var otherHas = otherEnumerator.MoveNext();
            if (!thisHas && otherHas)
            {
                ViewArray = (T[]) othersArray.Clone();
                return new List<MergeRecord> { new MergeRecord { StartIndex = 0, IsAdd = true, Items = ViewArray } };
            }
            if (thisHas && !otherHas)
            {
                var oldItems = ViewArray;
                ViewArray = new T[0];
                return new List<MergeRecord> { new MergeRecord { StartIndex = 0, IsAdd = false, Items = oldItems } };
            }
            var newArray = new T[othersArray.Count()];
            var index = 0;
            MergeRecord currentRecord = null;
            var result = new List<MergeRecord>();
            while (thisHas && otherHas)
            {
                var myItem = myEnumerator.Current;
                var otherItem = (T) otherEnumerator.Current;
                var compareResult = Comparison(myItem, otherItem);
                if (compareResult < 0)
                {
                    if (currentRecord == null || currentRecord.IsAdd)
                    {
                        var isAdd = currentRecord != null;
                        currentRecord = new MergeRecord();
                        result.Add(currentRecord);
                        currentRecord.Update(myItem, false, index + (isAdd ? 1 : 0));
                    } 
                    else currentRecord.Update(myItem);

                    thisHas = myEnumerator.MoveNext();
                }
                else if (compareResult > 0)
                {
                    if (currentRecord == null || !currentRecord.IsAdd)
                    {
                        currentRecord = new MergeRecord();
                        result.Add(currentRecord);
                        currentRecord.Update(otherItem, true, index);
                    }
                    else currentRecord.Update(otherItem);

                    newArray[index++] = otherItem;
                    otherHas = otherEnumerator.MoveNext();
                }
                else
                {
                    if (!Equals(myItem, otherItem))
                    {
                        if (currentRecord == null || !currentRecord.IsReplace)
                        {
                            currentRecord = new MergeRecord();
                            result.Add(currentRecord);
                            currentRecord.UpdateReplace(myItem, otherItem, index);
                        }
                        else currentRecord.UpdateReplace(myItem, otherItem);
                        newArray[index++] = otherItem;
                    }
                    else
                    {
                        currentRecord = null;
                        newArray[index] = otherItem;
                        index++;
                    }
                    thisHas = myEnumerator.MoveNext();
                    otherHas = otherEnumerator.MoveNext();
                }
            }
            // Se acabó el primero pero le queda al segundo
            if (otherHas)
            {
                if (currentRecord == null || currentRecord.IsReplace || !currentRecord.IsAdd)
                {
                    currentRecord = new MergeRecord { IsAdd = true, StartIndex = index };
                    result.Add(currentRecord);
                }
                while (otherHas)
                {
                    var current = (T) otherEnumerator.Current;
                    newArray[index++] = current;
                    currentRecord.Update(current);
                    otherHas = otherEnumerator.MoveNext();
                }
            }
            // Se acabó el segundo pero le queda al primero
            if (thisHas)
            {
                if (currentRecord == null || currentRecord.IsReplace || currentRecord.IsAdd)
                {
                    currentRecord = new MergeRecord { IsAdd = false, StartIndex = index };
                    result.Add(currentRecord);
                }
                while (thisHas)
                {
                    currentRecord.Update(myEnumerator.Current);
                    thisHas = myEnumerator.MoveNext();
                }
            }
            ViewArray = newArray;
            return result;
        }

        public void UpdateFromSort(Comparison<T> comparison, IEnumerable<T> newItems)
        {
            lock (this)
            {
                Comparison = comparison;
                if (ViewArray.Length <= 0)
                    return;
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ViewArray, 0));
                ViewArray = newItems.ToArray();
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ViewArray, 0));
            }
        }
    }

    public static class ObservableSortedViewExtensions
    {
        public static ObservableSortedView<T> ToObservableSortedView<T>(this IList<T> list, Comparison<T> comparison)
        {
            var result = new ObservableSortedView<T>(comparison);
            result.Update(list);
            return result;
        }
    }
}
