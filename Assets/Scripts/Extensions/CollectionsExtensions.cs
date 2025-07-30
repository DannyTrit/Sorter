using System;
using System.Collections.Generic;

namespace Sorter
{
    public static class CollectionsExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action?.Invoke(item);
            }
        }
    }
}