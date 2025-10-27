using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace SpanishPoint.Azure.Iswc.Data.Tests.TestFactories
{
    /// <summary>
    /// Mock DbSet factory class.
    /// </summary>
    public static class MockDbSetBuilder
    {
        /// <summary>
        /// Generic Mock DbSet builder.
        /// </summary>
        public static Mock<DbSet<T>> MockDbSetFactory<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}
