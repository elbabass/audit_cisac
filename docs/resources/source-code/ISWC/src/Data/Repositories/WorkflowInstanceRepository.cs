using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IWorkflowInstanceRepository : IRepository<WorkflowInstance>
    {
        Task<IEnumerable<WorkflowInstance>> FindWorkflows(Expression<Func<WorkflowInstance, bool>> predicate, int? startIndex, int? pageLength, IEnumerable<string> includes = null);
    }

    internal class WorkflowInstanceRepository : BaseRepository<WorkflowInstance>, IWorkflowInstanceRepository
    {
        public WorkflowInstanceRepository()
        {

        }
        public WorkflowInstanceRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }

        public async Task<IEnumerable<WorkflowInstance>> FindWorkflows(Expression<Func<WorkflowInstance, bool>> predicate, int? startIndex, int? pageLength, IEnumerable<string> includes = null)
        {
            int index = startIndex ?? 0;
            int length = pageLength ?? 200;

            var resultWithIncludes = includes == null
                ? context.Set<WorkflowInstance>()
                : includes.Aggregate(context.Set<WorkflowInstance>().AsQueryable(), (current, include) => current.Include(include));

            return await resultWithIncludes.Where(predicate).OrderByDescending(x => x.WorkflowInstanceId).Skip(index).Take(length).ToListAsync();
        }
    }
}
