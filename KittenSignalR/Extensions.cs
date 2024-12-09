using System.Collections.Generic;
using System;
using PictureAuth.Models;

namespace KittenSignalR
{
    public static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static bool IsAdmin(this RegisterDto user)
        {
            var adminNames = new List<string> { "admin", "administrator", "stephen", "root", "lindsey" };

            if (adminNames.Contains(user.Name.ToLower()))
            {
                return true;
            }

            return false;
        }
    }
}