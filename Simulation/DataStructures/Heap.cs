using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DataStructures
{
    /// <summary>
    /// Static heap data structure.
    /// </summary>
    public class Heap
    {
        private IComparable[] _items;
        private int _lastIndex = 0;
        public int Count { get => _lastIndex; }
        public Heap(int size)
        {
            _items = size > 0 ? new IComparable[1 << (int)Math.Ceiling(Math.Log2(size))] : new IComparable[0];
        }

        /// <summary>
        /// Adds an item to heap.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        public void Add(IComparable item)
        {
            if (_lastIndex == _items.Length)
            {
                throw new InvalidOperationException("Heap is full");
            }
            var end = false;
            var currentIndex = _lastIndex;
            _items[_lastIndex++] = item;
            while (currentIndex > 0 && !end)
            {
                end = true;
                if (_items[currentIndex].CompareTo(_items[(currentIndex - 1) / 2]) < 0)
                {
                    Swap((currentIndex - 1) / 2, currentIndex);
                    currentIndex = (currentIndex - 1) / 2;
                    end = false;
                }
            }
        }

        /// <summary>
        /// Gets the minimum value and removes it from the heap.
        /// </summary>
        /// <returns>Minimum value.</returns>
        public IComparable GetMinimum()
        {
            if (_lastIndex == 0)
            {
                return null;
            }
            var returnValue = _items[0];
            _items[0] = _items[--_lastIndex];
            var currentIndex = 0;
            var end = false;
            while (!end)
            {
                var smallestIndex = currentIndex;
                end = true;
                if ((currentIndex * 2) + 1 < _lastIndex && _items[(currentIndex * 2) + 1].CompareTo(_items[smallestIndex]) < 0)
                {
                    smallestIndex = (currentIndex * 2) + 1;
                    end = false;
                }
                if ((currentIndex * 2) + 2 < _lastIndex && _items[(currentIndex * 2) + 2].CompareTo(_items[smallestIndex]) < 0)
                {
                    smallestIndex = (currentIndex * 2) + 2;
                    end = false;
                }
                Swap(smallestIndex, currentIndex);
                currentIndex = smallestIndex;
            }

            return returnValue;
        }

        public IComparable Peak()
        {
            IComparable result = null;
            if (_lastIndex > 0)
            {
                result = _items[0];
            }
            return result;
        }

        private void Swap(int index1, int index2)
        {
            IComparable temp = _items[index1];
            _items[index1] = _items[index2];
            _items[index2] = temp;
        }
    }
}
