using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RecipeMaster.Util
{
    /// <summary>
    /// Class for a collection that is the result of a many-to-many join. 
    /// </summary>
    /// <typeparam name="T">Type of the associated items</typeparam>
    /// <typeparam name="TSelf">Type of the collection's owner</typeparam>
    /// <typeparam name="TJoin">Join class type</typeparam>
    public class JoinCollection<T, TSelf, TJoin> : ICollection<T> where TJoin : Nullifyable
    {
        /// <summary>
        /// Object that owns the collection of join-class items
        /// </summary>
        private TSelf Owner;
        /// <summary>
        /// The collection of join-class items
        /// </summary>
        private ICollection<TJoin> SourceCollection { get; set; }
        /// <summary>
        /// Function that transforms a join-class item into a result type item
        /// </summary>
        private Func<TJoin, T> ResultFromJoin { get; set; }
        /// <summary>
        /// The collection of result type item (transformed from the join-class collection)
        /// </summary>
        private ICollection<T> ResultCollection { get => SourceCollection.Select(ResultFromJoin).ToList(); }
        /// <summary>
        /// Function that creates a new join-class item 
        /// </summary>
        private Func<T, TSelf, TJoin> NewJoinItem { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">Object calling this constructor</param>
        /// <param name="sourceCollection">Collection of join-class items</param>
        /// <param name="resultFromJoin">Function that transforms a join-class item into a result type item</param>
        /// <param name="newJoinItem">Function that creates a new join-class item </param>
        public JoinCollection(
            TSelf owner,
            ICollection<TJoin> sourceCollection,
            Func<TJoin, T> resultFromJoin,
            Func<T, TSelf, TJoin> newJoinItem
        )
        {
            Owner = owner;
            SourceCollection = sourceCollection;
            ResultFromJoin = resultFromJoin;
            NewJoinItem = newJoinItem;
        }

        /// <inheritdoc/>
        public int Count => SourceCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to collection (by adding a new join-class item to the collection of join-class items)
        /// </summary>
        /// <param name="item">Item to be added</param>
        void ICollection<T>.Add(T item)
        {
            // Prevent duplicate item addition
            if (ResultCollection.Contains(item)) throw new Exception("Item already in collection");
            // Add a new join item to the source collection
            SourceCollection.Add(NewJoinItem(item, Owner));
        }
        /// <summary>
        /// Adds an item to collection (by adding a new join-class item to the collection of join-class items)
        /// </summary>
        /// <param name="item">Item to be added</param>
        /// <returns>The new join-class item</returns>
        public TJoin Add(T item)
        {
            if (ResultCollection.Contains(item)) throw new Exception("Item already in collection");
            // Add a new join item to the source collection
            TJoin joinItem = NewJoinItem(item, Owner);
            SourceCollection.Add(joinItem);
            return joinItem;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (T item in ResultCollection)
            {
                Remove(item);
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => ResultCollection.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => ResultCollection.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => ResultCollection.GetEnumerator();

        /// <summary>
        /// Removes an item from the collection (marks the corresponding join-class item for deletion by nullifying it)
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>True if the item was removed, false if the item was not removed (because it wasn't in the collection)</returns>
        public bool Remove(T item)
        {
            if (item == null) return false;
            // Get the join item
            TJoin joinItem = SourceCollection.FirstOrDefault(ji => item.Equals(ResultFromJoin(ji)));
            if (joinItem == null)
            {
                return false;
            }
            // Set join item props to null so database will know to delete it
            joinItem.Nullify();
            return true;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => SourceCollection.GetEnumerator();
    }

    /// <summary>
    /// A collection that is the result of a many-to-1 join
    /// </summary>
    /// <typeparam name="T">Type of the assocaited items (the many)</typeparam>
    /// <typeparam name="Tself">Type of the collection's owner (the 1)</typeparam>
    public class JoinCollection<T, Tself> : ICollection<T>
    {
        /// <summary>
        /// Object that owns the collection
        /// </summary>
        private Tself Owner;
        /// <summary>
        /// The owner's private collection of tems
        /// </summary>
        private ICollection<T> SourceCollection { get; set; }
        /// <summary>
        /// Action to set an associated item's reciprical property
        /// </summary>
        private Action<T, Tself> SetRecipricalProperty;
        /// <summary>
        /// Action to unset an associated item's reciprical property
        /// </summary>
        private Action<T> UnsetRecipricalProperty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">Object which owns this collection</param>
        /// <param name="sourceCollection">Private collection of items</param>
        /// <param name="setRecipricalProperty">Action to set an associated item's reciprical property</param>
        /// <param name="unsetRecipricalProperty">Action to unset an associated item's reciprical property</param>
        public JoinCollection(
            Tself owner,
            ICollection<T> sourceCollection,
            Action<T, Tself> setRecipricalProperty,
            Action<T> unsetRecipricalProperty)
        {
            Owner = owner;
            SourceCollection = sourceCollection;
            SetRecipricalProperty = setRecipricalProperty;
            UnsetRecipricalProperty = unsetRecipricalProperty;
        }

        /// <inheritdoc/>
        public int Count => SourceCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the collection, and sets the reciprical property on that item
        /// </summary>
        /// <param name="item">Item to be added</param>
        public void Add(T item)
        {
            // Prevent duplicate item addition
            if (SourceCollection.Contains(item)) throw new Exception("Item already in collection");

            // Add a new join item to the source collection
            SourceCollection.Add(item);

            // Set the reciprical property on the item to be the owner
            SetRecipricalProperty(item, Owner);
        }

        /// <summary>
        /// Removes all items from the collection, and unsets the reciprical property on each item
        /// </summary>
        public void Clear()
        {
            foreach (T item in SourceCollection)
            {
                Remove(item);
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => SourceCollection.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => SourceCollection.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => SourceCollection.GetEnumerator();

        /// <summary>
        /// Removes an items from the collection, and unsets the reciprical property on that item
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns>True if the item was removed, false if the item was not removed (because it wasn't in the collection)</returns>
        public bool Remove(T item)
        {
            // Remove from owner's collection, if possibe
            if (SourceCollection.Remove(item))
            {
                // Set the reciprical property on the item to be null
                UnsetRecipricalProperty(item);
                return true;
            }
            // Otherwise, there was nothing to remove
            return false;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => SourceCollection.GetEnumerator();
    }
}
