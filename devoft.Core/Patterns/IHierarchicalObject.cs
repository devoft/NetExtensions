using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace devoft.Core.Patterns
{
    public interface IHierarchicalObject
    {
        object Parent { get; }
        IEnumerable Children { get; }
    }

    public interface IHierarchicalObject<T> : IHierarchicalObject
        where T : IHierarchicalObject<T>
    {
        new T Parent { get; }
        new IEnumerable<T> Children { get; }
    }

    public class HierarchicalObject<T> : IHierarchicalObject<T>
        where T : HierarchicalObject<T>
    {
        protected T _parent;

        public virtual T Parent
        {
            get => _parent;
            set
            {
                var oldParent = _parent;
                if (Equals(oldParent, value))
                    return;

                if (oldParent != null)
                    ((IList<T>)oldParent.Children).Remove((T) this);

                _parent = value;

                if (_parent != null)
                    ((IList<T>)_parent.Children).Add((T) this);
            }
        }

        public virtual IEnumerable<T> Children { get; } = new List<T>();

        object IHierarchicalObject.Parent => Parent;

        IEnumerable IHierarchicalObject.Children => Children;
    }


}
