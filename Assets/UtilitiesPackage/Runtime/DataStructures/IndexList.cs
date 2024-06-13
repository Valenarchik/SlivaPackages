using System.Collections;
using System.Collections.Generic;
using System;

namespace DataStructures
{
    public class IndexList<T> : IReadOnlyIndexList<T>
    {
        private int lastIndex;
        private readonly Dictionary<int, T> indexToObject;
        private readonly Dictionary<T, int> objectToIndex;
        private readonly HashSet<int> indexesSet;

        public IReadOnlyDictionary<T, int> ObjectToIndex => objectToIndex;
        public IReadOnlyDictionary<int, T> IndexToObject => indexToObject;

        public int Count => indexesSet.Count;
        public IEnumerable<int> Keys => indexesSet;
        public IEnumerable<T> Values => indexToObject.Values;

        public IndexList()
        {
            lastIndex = 0;
            indexToObject = new Dictionary<int, T>();
            objectToIndex = new Dictionary<T, int>();
            indexesSet = new HashSet<int>();
        }

        public IndexList(IReadOnlyDictionary<int, T> dict) : this()
        {
            var maxIndex = int.MinValue;
            foreach (var pair in dict)
            {
                indexToObject[pair.Key] = pair.Value;
                objectToIndex[pair.Value] = pair.Key;
                maxIndex = Math.Max(maxIndex, pair.Key);
            }

            lastIndex = maxIndex + 1;
        }

        public int Add(T item)
        {
            var index = lastIndex;
            Add(index, item);
            lastIndex++;
            while (indexesSet.Contains(lastIndex))
                lastIndex++;

            return index;
        }

        public void Add(int index, T item)
        {
            if (indexesSet.Contains(index))
                throw new ArgumentException("Index is already taken!");

            indexToObject[index] = item;
            objectToIndex[item] = index;
            indexesSet.Add(index);
        }

        public void Remove(T item)
        {
            var index = objectToIndex[item];
            indexToObject.Remove(index);
            objectToIndex.Remove(item);
        }

        public void Remove(int index)
        {
            var obj = indexToObject[index];
            Remove(obj);
        }

        public void Clear()
        {
            lastIndex = 0;
            indexToObject.Clear();
            objectToIndex.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface IReadOnlyIndexList<T>: IEnumerable<T>
    {
        public IReadOnlyDictionary<int, T> IndexToObject { get; }
        public IReadOnlyDictionary<T, int> ObjectToIndex { get; }

        public int Count { get; }

        public IEnumerable<int> Keys { get; }
        public IEnumerable<T> Values { get; }
    }
}