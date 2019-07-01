using devoft.System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace devoft.System.Collections.Specialized
{
    /// <summary>
    /// Colección cuyo contenido se actualiza mediante consultas y que notifica los cambios
    /// correspondientes al mecanismo de Binding de WPF.
    /// </summary>
    /// <typeparam name="T">Tipo de cada elemento de la colección</typeparam>
    public class ObservableView<T> : ObservableViewBase<T>, IList<T>
    {
        public ObservableView()
        {
        }

        public ObservableView(IEnumerable<T> col)
        {
            Update(col);
        }

        public ObservableView(params T[] col)
        {
            Update(col);
        }

        public bool IsReadOnly => false;

        T IList<T>.this[int index]
        {
            get { return ViewArray[index]; }
            set { ViewArray[index] = value; }
        }

        public bool Remove(T item)
        {
            try
            {
                Update(ViewArray.Remove(item));
                return true;
            } catch
            {
                return false;
            }
        }

        public void Add(T item)
        {
            Update(ViewArray.Add(item));
        }

        protected override IEnumerable<MergeRecord> UpdateOverride(IEnumerable<T> others)
        {
            var othersList = others.ToList();
            List<int> existingInOthers;
            var remainings = ViewArray.ToList();
            var result = RemoveOldValues(remainings, othersList, out existingInOthers);
            if (!existingInOthers.IsSorted(new Comparison<int>((x, y) => x - y).ToComparer(), true))
                result.AddRange(Reorder(remainings, existingInOthers));
            result.AddRange(InsertNewValues(othersList, existingInOthers));
            ViewArray = othersList.ToArray();
            return result;
        }

        private IEnumerable<MergeRecord> InsertNewValues(List<T> othersList, List<int> existingInOthers)
        {
            existingInOthers.Sort();
            var previousIndex = 0;
            foreach (var index in existingInOthers)
            {
                if (index != previousIndex)
                    yield return new MergeRecord
                    {
                        IsAdd = true,
                        Items = othersList.GetRange(previousIndex, index - previousIndex).ToList(),
                        StartIndex = previousIndex
                    };
                previousIndex = index + 1;
            }
            if (previousIndex < othersList.Count)
                yield return new MergeRecord
                    {
                        IsAdd = true,
                        Items = othersList.GetRange(previousIndex, othersList.Count - previousIndex).ToList(),
                        StartIndex = previousIndex
                    };
        }

        private IEnumerable<MergeRecord> Reorder(List<T> remainings, IEnumerable<int> existingInOthers)
        {
            var items = remainings.ToList();
            remainings.Clear();
            var indexes = existingInOthers.ToList();
            var maxIndex = 0;
            var count = items.Count;
            var result = new List<MergeRecord>();
            for (var k = 0; k < count; k++)
            {
                var localMax = int.MinValue;
                for (var i = 0; i < indexes.Count; i++)
                    if (indexes[i] > localMax)
                    {
                        maxIndex = i;
                        localMax = indexes[i];
                    }
                result.Add(MergeRecord.ForMove(items[maxIndex], remainings.Count + maxIndex, 0));
                remainings.Insert(0, items[maxIndex]);
                items.RemoveAt(maxIndex);
                indexes.RemoveAt(maxIndex);
            }
            return result;
        }

        private static List<MergeRecord> RemoveOldValues(List<T> remainings, List<T> others, out List<int> existingPositions)
        {
            var result = new List<MergeRecord>();
            var i = 0;
            var dictionary = others.SafeToDictionary(item => item, item => i++);
            var othersSet = others.ToHashSet();
            existingPositions = new List<int>();
            MergeRecord currentMergeRecord = null;
            var index = 0;
            var array = remainings.ToArray();
            remainings.Clear();
            foreach (var item in array)
            {
                if (!othersSet.Contains(item))
                {
                    if (currentMergeRecord == null)
                    {
                        currentMergeRecord = new MergeRecord();
                        currentMergeRecord.Update(item, false, index++);
                        result.Add(currentMergeRecord);
                    }
                    else
                    {
                        currentMergeRecord.Update(item);
                        index++;
                    }
                }
                else
                {
                    existingPositions.Add(dictionary[item]);
                    remainings.Add(item);
                    index++;
                    if (currentMergeRecord != null)
                    {
                        index -= currentMergeRecord.Items.Count;
                        currentMergeRecord = null;
                    }
                }
            }
            return result;
        }

        public bool Contains(T item)
            => ViewArray.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => ViewArray.CopyTo(array, arrayIndex);

        public int IndexOf(T item)=> ViewArray.IndexOf(item);

        public void Insert(int index, T item)
            => Update(ViewArray.Insert(index, item));

        public void RemoveAt(int index)
            => Update(ViewArray.RemoveAt(index));
    }
}