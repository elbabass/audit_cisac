using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class QueryableExtensions
    {
        public static IQueryable<T> IncludeAll<T, TProperty>(this IQueryable<T> source, params Expression<Func<T, TProperty>>[] paths) 
            where T : class 
            where TProperty : class => 
            paths.Aggregate(source, (current, path) => current.IncludeOptimized(path));

        public static IQueryable<T> IncludeAllByPath<T>(this IQueryable<T> source, params string[] paths)
            where T : class => 
            paths.Aggregate(source, (current, path) => current.IncludeOptimizedByPath(path));
    }
}
