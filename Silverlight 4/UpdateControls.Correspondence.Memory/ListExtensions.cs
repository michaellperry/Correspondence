using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Memory
{
    public static class ListExtensions
    {
        public static int RemoveAll<T>(this List<T> list, Func<T, bool> filter)
        {
            int removed = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (filter(list[i]))
                {
                    list.RemoveAt(i);
                    i--;
                    ++removed;
                }
            }
            return removed;
        }
    }
}
