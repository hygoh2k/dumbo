using System;
using System.Collections;
using System.Collections.Generic;

namespace Dombo.Extension
{
    public static class CollectionExtension
    {
        //extention to add range
         public static void AddRangeEx<T>(this IList<T> list, T[] value)
        {
            foreach (var item in value)
            {
                list.Add(item);
            }

        }

    }


    public static class IntExtensions
    {
        public static bool IsGreaterThan(this int i, int value)
        {
            return i > value;
        }
    }
}
