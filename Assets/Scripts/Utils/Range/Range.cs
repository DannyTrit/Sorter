using UnityEngine;

namespace Sorter.Utils
{
    public abstract class Range<T>
    {
        [field: SerializeField] public T Min { get; private set; }
        [field: SerializeField] public T Max { get; private set; }

        public abstract T Clamp(T value);
        public abstract bool Contains(T value);
    }
}