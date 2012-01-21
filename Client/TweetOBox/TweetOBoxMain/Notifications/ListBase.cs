﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TweetOBoxMain.Notifications;

namespace TweetOBoxMain.Notifications
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Serves as a base implemetation of <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item in the list.</typeparam>
    //////////////////////////////////////////////////////////////////////////
    public abstract class ListBase<T> : IList<T>, IList
    {
        #region IList<T> Members

        //--------------------------------------------------------------------
        /// <summary>
        ///     Searches for the specified object and returns the zero-based
        ///     index of the first occurrence within the entire <see cref="ListBase{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="ListBase{T}"/>. 
        ///     The value can be null for reference types.</param>
        /// <returns>
        ///     The zero-based index of the first occurrence of item within the 
        ///     entire <see cref="ListBase{T}"/>, if found; otherwise, –1.
        /// </returns>
        //--------------------------------------------------------------------
        public virtual int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     Insert is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        public virtual void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     RemoveAt is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        public virtual void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        //--------------------------------------------------------------------
        public T this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     When overridden in a derived class, provides the item at the 
        ///     specified index;
        /// </summary>
        /// <param name="index">The index of the desired item.</param>
        /// <returns>The item at the provided index.</returns>
        //--------------------------------------------------------------------
        protected abstract T GetItem(int index);

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     SetItem is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        protected virtual void SetItem(int index, T value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T> Members

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     Add is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        public virtual void Add(T item)
        {
            throw new NotSupportedException();
        }

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     Clear is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        public virtual void Clear()
        {
            throw new NotSupportedException();
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     Determines whether an element is in the list.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the list.
        ///     The value can be null for reference types.
        /// </param>
        /// <returns>
        ///     true if item is found in the list; otherwise, false.
        /// </returns>
        //--------------------------------------------------------------------
        public virtual bool Contains(T item)
        {
            if (item == null)
            {
                for (int num1 = 0; num1 < this.Count; num1++)
                {
                    if (this[num1] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            EqualityComparer<T> comparer1 = EqualityComparer<T>.Default;
            for (int num2 = 0; num2 < this.Count; num2++)
            {
                if (comparer1.Equals(this[num2], item))
                {
                    return true;
                }
            }
            return false;
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     Copies the entire list to a compatible one-dimensional array,
        ///     starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional Array that is the destination of the elements copied from list.
        ///     The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        //--------------------------------------------------------------------
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo((Array)array, arrayIndex);
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     When overridden in a derived class, returns the number of items
        ///     in the list.
        /// </summary>
        /// <remarks>The number of items in the list.</remarks>
        //--------------------------------------------------------------------
        public abstract int Count
        {
            get;
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     Gets a value representing if the List is read-only.
        /// </summary>
        /// <remarks>true if the list is read-only; otherwise, false.</remarks>
        //--------------------------------------------------------------------
        public virtual bool IsReadOnly
        {
            get { return true; }
        }

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     Remove is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        public virtual bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        //--------------------------------------------------------------------
        /// <summary>
        ///     Returns an enumerator that iterates through the list.
        /// </summary>
        //--------------------------------------------------------------------
        public virtual IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            VerifyValueType(value);
            this.Add((T)value);
            return (this.Count - 1);
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return this.Contains((T)value);
            }
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return this.IndexOf((T)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            VerifyValueType(value);
            this.Insert(index, (T)value);
        }

        //--------------------------------------------------------------------
        /// <summary>
        ///     Gets a value indicating whether the list has a fixed size.
        /// </summary>
        //--------------------------------------------------------------------
        public virtual bool IsFixedSize
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                this.Remove((T)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                VerifyValueType(value);
                this[index] = (T)value;
            }
        }

        #endregion

        #region ICollection Members

        //--------------------------------------------------------------------
        /// <summary>
        ///     Copies the entire <see cref="ListBase{T}"/> to a compatible one-dimensional 
        ///     array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional Array that is the destination of the elements copied from <see cref="ListBase{T}"/>.
        ///     The Array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        //--------------------------------------------------------------------
        public virtual void CopyTo(Array array, int index)
        {
            Utils.RequireNotNull(array, "array");
            Utils.RequireArgument(array.Rank == 1, "array", "Array must be 1-dimentional.");
            Utils.RequireArgumentRange(array.Length >= (this.Count + index), "index");

            for (int i = 0; i < this.Count; i++)
            {
                array.SetValue(this[i], index + i);
            }
        }

        //--------------------------------------------------------------------
        /// <exception cref="NotSupportedException">
        ///     IsSynchronized is not directly supported. To add support,
        ///     override in a subclass.
        /// </exception>
        //--------------------------------------------------------------------
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                if (m_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref m_syncRoot, new object(), null);
                }
                return m_syncRoot;
            }
        }

        #endregion

        #region Private Static Helpers

        private static bool IsCompatibleObject(object value)
        {
            if (!(value is T) && ((value != null) || typeof(T).IsValueType))
            {
                return false;
            }
            return true;
        }

        [DebuggerStepThrough]
        private static void VerifyValueType(object value)
        {
            if (!IsCompatibleObject(value))
            {
                throw new ArgumentException("value");
            }
        }

        #endregion

        private object m_syncRoot;

    } //*** class ListBase<T>

} 
