using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesAndRemindersLibrary.Helpers
{
    public static class IEnumerableExtensions
    {
        public static int FindIndexForSortedInsert<T>(this IEnumerable<T> items, T inserting, IComparer<T> comparer)
        {
            int i = 0;

            foreach (var item in items)
            {
                if (comparer.Compare(inserting, item) < 0)
                    return i;

                i++;
            }

            return i;
        }
    }
}
