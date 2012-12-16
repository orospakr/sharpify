using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary
{
    public interface UntypedDirtiableList
    {
        void Reset();
    }

    public class DirtiableList<T> : IList<T>, IDirtiable, UntypedDirtiableList where T : IDirtiable
    {
        private bool _Dirty;
        private List<T> _Wrapped;

        public DirtiableList()
        {
            _Dirty = true;
            _Wrapped = new List<T>();
        }

        public void Reset()
        {
            _Dirty = false;
        }

        public bool IsClean()
        {
            if (_Dirty)
            {
                return false;
            }
            var dirties = from p in this where !p.IsClean() select p;
            return dirties.Count() == 0;
        }

        public int IndexOf(T item)
        {
            return _Wrapped.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _Wrapped.Insert(index, item);
            _Dirty = true;
        }

        public void RemoveAt(int index)
        {
            _Wrapped.RemoveAt(index);
            _Dirty = true;
        }

        public T this[int index]
        {
            get
            {
                return _Wrapped[index];
            }
            set
            {
                _Wrapped[index] = value;
            }
        }

        public void Add(T item)
        {
            _Wrapped.Add(item);
            _Dirty = true;
        }

        public void Clear()
        {
            _Wrapped.Clear();
            _Dirty = true;
        }

        public bool Contains(T item)
        {
            return _Wrapped.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _Wrapped.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Wrapped.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            _Dirty = true;
            return _Wrapped.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Wrapped.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_Wrapped).GetEnumerator();
        }
    }
}
