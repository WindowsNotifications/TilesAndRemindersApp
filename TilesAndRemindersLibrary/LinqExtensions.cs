using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesAndRemindersLibrary
{
    public static class LinqExtensions
    {
        public static int RemoveAll<T>(this IList<T> collection, Predicate<T> match)
        {
            int countOfRemoved = 0;

            for (int i = 0; i < collection.Count; i++)
            {
                if (match(collection[i]))
                {
                    collection.RemoveAt(i);
                    i--;
                    countOfRemoved++;
                }
            }

            return countOfRemoved;
        }
    }
}
