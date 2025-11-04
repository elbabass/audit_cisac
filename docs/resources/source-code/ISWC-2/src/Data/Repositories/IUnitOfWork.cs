using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IUnitOfWork
    {
        Task Save();
        IExecutionStrategy CreateExecutionStrategy();
        IDbContextTransaction BeginTransaction();
    }
}
