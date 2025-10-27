using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector)
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
    }
}
