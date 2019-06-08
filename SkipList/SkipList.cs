using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SkipList
{
    public class SkipList<T> : ICollection<T>, IEnumerable<T>
    {
        internal class Node
        {
            public T Value;
            public Node[] Neighbors;
            public int Height => Neighbors.Length;

            public Node this[int index]
            {
                get { return Neighbors[index]; }
                set { Neighbors[index] = value; }
            }

            public Node(int height) : this(default(T), height) { }
            public Node(T value, int height)
            {
                Value = value;
                Neighbors = new Node[height];
            }

            public void IncrementHeight()
            {
                var temp = new Node[Height + 1];
                for (int i = 0; i < Height; i++)
                {
                    temp[i] = Neighbors[i];
                }

                Neighbors = temp;
            }
        }

        internal Node head { get; set; }
        internal Random rand { get; set; } = new Random();
        public IComparer<T> Comparer { get; private set; }

        public SkipList() : this((IComparer<T>)null) { }

        public SkipList(IComparer<T> comparer)
        {
            Count = 0;
            head = new Node(1);
            Comparer = comparer ?? Comparer<T>.Default;
        }

        public SkipList(IEnumerable<T> collection) : this(collection, null) { }

        public SkipList(IEnumerable<T> collection, IComparer<T> comparer) : this(comparer)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public int Count { get; set; } = 0;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            Node temp = new Node(item, ChooseRandomHeight());

            if (temp.Height > head.Height)
            {
                head.IncrementHeight();
            }

            Node curr = head;
            int level = head.Height - 1;

            while (level >= 0)
            {
                int comparison = curr[level] == null ? 1 : Comparer.Compare(curr[level].Value, item);

                if (comparison > 0)
                {
                    if (temp.Height > level)
                    {
                        temp[level] = curr[level];
                        curr[level] = temp;
                    }
                    level--;
                }

                else if (comparison < 0)
                {
                    curr = curr[level];
                }

                else
                {
                    throw new Exception("Duplicate value exists.");
                }
            }

            Count++;
        }

        public bool Remove(T item)
        {
            bool removal = false;
            Node curr = head;
            int level = head.Height - 1;

            while (level > -1)
            {
                int comparison = curr[level] == null ? 1 : Comparer.Compare(curr[level].Value, item);

                if (comparison > 0)
                {
                    level--;
                }

                else if (comparison < 0)
                {
                    curr = curr[level];
                }

                else
                {
                    removal = true;
                    curr[level] = curr[level][level];
                    level--;
                }

                if (removal)
                {
                    Count--;
                }
            }
            return removal;
        }

        public void Clear()
        {
            head = new Node(1);
        }

        public bool Contains(T item)
        {
            Node curr = head;
            int level = head.Height - 1;

            while (level > -1)
            {
                int comparison = curr[level] == null ? 1 : Comparer.Compare(curr[level].Value, item);

                if (comparison > 0)
                {
                    level--;
                }
                else if (comparison < 0)
                {
                    curr = curr[level];
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo(array, index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
            }
            if (array.Length < Count + arrayIndex)
            {
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
            }
            foreach (T value in this)
            {
                array[arrayIndex] = value;
                arrayIndex++;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var curr = head;
            while (curr[0] != null)
            {
                yield return curr[0].Value;
                curr = curr[0];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int ChooseRandomHeight()
        {
            int height = 1;
            while (CoinFlip() == 1 && head.Height <= height + 1)
            {
                height++;
            }
            return height;
        }

        internal int CoinFlip()
        {
            return rand.Next(0, 2);
        }
    }
}
