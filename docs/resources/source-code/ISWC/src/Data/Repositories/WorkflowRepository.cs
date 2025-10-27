using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
	public interface IWorkflowRepository : IRepository<WorkflowTask>
	{
        Task<IEnumerable<WorkflowTask>> FindWorkflowTasks(Expression<Func<WorkflowTask, bool>> predicate, int? startIndex, int? pageLength, IEnumerable<string> includes = null);
    }
	internal class WorkflowRepository : BaseRepository<WorkflowTask>, IWorkflowRepository
	{
		public WorkflowRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }

		public async Task<IEnumerable<WorkflowTask>> FindWorkflowTasks(Expression<Func<WorkflowTask, bool>> predicate, int? startIndex, int? pageLength, IEnumerable<string> includes = null)
		{
			int index = startIndex ?? 0;
			int length = pageLength ?? 1000;

			var resultWithIncludes = includes == null
				? context.Set<WorkflowTask>()
				: includes.Aggregate(context.Set<WorkflowTask>().AsQueryable(), (current, include) => current.Include(include));

			return await resultWithIncludes.Where(predicate).Skip(index).Take(length).ToListAsync();
		}
    }
}
