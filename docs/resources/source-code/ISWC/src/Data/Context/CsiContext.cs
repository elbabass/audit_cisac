using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SpanishPoint.Azure.Iswc.Data.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace SpanishPoint.Azure.Iswc.Data.DataModels.Context
{
    public class CsiContext : CsiBaseContext, IUnitOfWork
    {
        public CsiContext(DbContextOptions options) : base(options)
        {
        }

        public async Task Save() => await SaveChangesAsync();
        public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();
        public IDbContextTransaction BeginTransaction() => Database.BeginTransaction(IsolationLevel.ReadCommitted);
    }
}
