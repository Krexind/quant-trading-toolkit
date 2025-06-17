using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuantitativeTrading.Data.DataProviders
{
    public abstract class KlineDataProvider<T, U> : IEnumerable<T>
        where U : KlineDataProvider<T, U>
    {
        public bool IsEnd => Index >= models.Length - 1;
        public T this[int Index] => models[Index];
        public T Current => models[Index];
        public long Length => models.Length;

        protected T[] models;

        public int Index { get; protected set; } = 0;

        public void Reset() => Index = 0;

        public bool MoveNext(out T model)
        {
            Index++;
            if (IsEnd)
            {
                model = default;
                return false;
            }
            
            model = models[Index];
            return true;
        }

        public IEnumerable<T> GetHistory(int historyPoint)
        {
            int timePoint = historyPoint - 1;
            int historyIndex = 0;
            if (Index > timePoint)
                historyIndex = Index - timePoint;
            for (int i = historyIndex; i <= Index; i++)
                yield return models[i];
        }

        public abstract U Clone();

        public abstract U Clone(int startIndex, int length);

        public abstract U CloneAllStatus();

        public IEnumerator<T> GetEnumerator()
            => models.Cast<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => models.GetEnumerator();
    }
}
