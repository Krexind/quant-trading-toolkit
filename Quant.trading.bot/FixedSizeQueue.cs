using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading
{
    public class FixedSizeQueue<T> : Queue<T>
    {
        private T[] queueArray;

        public int Size { get; private set; }
        public T this[int index] => queueArray[index];
        public T First => queueArray[0];
        public T Last => queueArray[Count - 1];

        public FixedSizeQueue(int size)
            => (queueArray, Size) = (new T[size], size);

        public void Resize(int size)
        {
            Size = size;
            DequeueWhenFull();
            queueArray = new T[size];
            CopyTo(queueArray, 0);
        }

        public new void Enqueue(T item)
        {
            DequeueWhenFull();
            // To enqueue
            base.Enqueue(item);

            //  這邊有Bug
            CopyTo(queueArray, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DequeueWhenFull()
        {
            while (Count >= Size)
                Dequeue();
        }
    }
}
