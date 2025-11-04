using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IWorkRepository : IRepository<WorkInfo>
    {
        void DeleteWork(WorkInfo workInfo);
        Task<WorkInfo> FindAsyncOptimized(Expression<Func<WorkInfo, bool>> predicate, bool creatorOnly);
        Task<IEnumerable<WorkInfo>> FindManyAsyncUnionAll(IEnumerable<WorkNumber> workNumbers);
        Task<bool> IsArchivedIswc(string preferredIswc);
        Task<long> FindArchivedIswc(string archivedIswc);
        Task<bool> ExistAsync(ICollection<WorkNumber> workNumbers, bool allExist);
        Task<IEnumerable<WorkInfo>> FindManyWorkInfosAsync(ICollection<WorkNumber> workNumbers);
        Task<IEnumerable<WorkInfo>> FindManyWorkInfosAsync(IEnumerable<long> workInfosIds);
    }

    internal class WorkRepository : BaseRepository<WorkInfo>, IWorkRepository
    {
        public WorkRepository()
        {

        }
        public WorkRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }

        public void DeleteWork(WorkInfo workInfo)
        {
            context.Title.RemoveRange(workInfo.Title);
            context.Creator.RemoveRange(workInfo.Creator);
            context.Publisher.RemoveRange(workInfo.Publisher);
            context.WorkInfoPerformer.RemoveRange(workInfo.WorkInfoPerformer);
            context.DerivedFrom.RemoveRange(workInfo.DerivedFrom);

            foreach (var ai in workInfo.AdditionalIdentifier)
                if (ai.Recording != null){
                    context.Recording.Remove(ai.Recording);
                }

            context.AdditionalIdentifier.RemoveRange(workInfo.AdditionalIdentifier);

            foreach (var wf in workInfo.WorkflowInstance)
                context.WorkflowTask.RemoveRange(wf.WorkflowTask);

            context.WorkflowInstance.RemoveRange(workInfo.WorkflowInstance);
            context.WorkInfo.Remove(workInfo);
        }

        public async Task<WorkInfo> FindAsyncOptimized(Expression<Func<WorkInfo, bool>> predicate, bool creatorOnly)
        {
            var queryable = context.Set<WorkInfo>().AsQueryable().Where(predicate)
                .Where(x => x.Status)
                .IncludeFilter(x => x.Iswc)
                .IncludeFilter(x => x.Creator.Where(y => y.IswcId == x.IswcId));

            if (!creatorOnly)
                queryable = queryable
                    .IncludeFilter(x => x.Agency)
                    .IncludeFilter(x => x.Publisher.Where(y => y.IswcId == x.IswcId))
                    .IncludeFilter(x => x.Title.Where(y => y.IswcId == x.IswcId));

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WorkInfo>> FindManyWorkInfosAsync(ICollection<WorkNumber> workNumbers)
        {
            var queryable = context.Set<WorkInfo>().AsQueryable()
                .Where(x => workNumbers.Select(x => x.Number).Contains(x.AgencyWorkCode))
                .Where(x => workNumbers.Select(x => x.Type).Contains(x.AgencyId))
                .Where(x => x.Status && !x.IsReplaced)
                .IncludeFilter(x => x.Iswc)
                .IncludeFilter(x => x.Creator.Where(y => y.WorkInfoId == x.WorkInfoId))
                .IncludeFilter(x => x.Agency)
                .IncludeFilter(x => x.Publisher.Where(y => y.WorkInfoId == x.WorkInfoId))
                .IncludeFilter(x => x.Title.Where(y => y.WorkInfoId == x.WorkInfoId))
                .IncludeFilter(x => x.WorkInfoPerformer.Where(y => y.WorkInfoId == x.WorkInfoId).Select(x => x.Performer))
                .IncludeFilter(x => x.DerivedFrom.Where(y => y.WorkInfoId == x.WorkInfoId))
                .IncludeFilter(x => x.DisambiguationIswc.Where(y => y.WorkInfoId == x.WorkInfoId))
                .IncludeFilter(x => x.AdditionalIdentifier.Where(y => y.WorkInfoId == x.WorkInfoId));

            return await queryable.ToListAsync();
        }

        public async Task<IEnumerable<WorkInfo>> FindManyWorkInfosAsync(IEnumerable<long> workInfoIds)
        {
            var queryable = context.Set<WorkInfo>().AsQueryable()
                .Where(x => workInfoIds.Contains(x.WorkInfoId))
                .Where(x => x.Status && !x.IsReplaced)
                .Include(x => x.Iswc)
                .Include(x => x.Creator)
                    .ThenInclude(c => c.IpnameNumberNavigation)
                .Include(x => x.Agency)
                .Include(x => x.Publisher)
                    .ThenInclude(p => p.IpnameNumberNavigation)
                .Include(x => x.Title)
                .Include(x => x.AdditionalIdentifier);

            return await queryable.ToListAsync();

        }

        public async Task<bool> ExistAsync(ICollection<WorkNumber> workNumbers, bool allExist)
        {
            var res = await context.Set<WorkInfo>()
                 .Where(x => workNumbers.Select(x => x.Number).Contains(x.AgencyWorkCode))
                 .Where(x => workNumbers.Select(x => x.Type).Contains(x.AgencyId))
                 .Where(x => x.Status).Select(x => new { x.AgencyWorkCode, x.AgencyId }).ToListAsync();

            if (allExist)
                return res.Any() && workNumbers.Where(y => res.Any(x => x.AgencyWorkCode == y.Number && y.Type == x.AgencyId)).Count() == workNumbers.Count;

            return res.Any() && res.Any(x => workNumbers.Any(y => y.Number == x.AgencyWorkCode && y.Type == x.AgencyId));
        }

        public async Task<IEnumerable<WorkInfo>> FindManyAsyncUnionAll(IEnumerable<WorkNumber> workNumbers)
        {
            IEnumerable<WorkInfo> returnList = Enumerable.Empty<WorkInfo>();

            foreach (var batch in workNumbers.DistinctBy(x => x.Number).Batch(2000))
            {
                var list = new List<IQueryable<WorkInfo>>();

                foreach (var workNumber in workNumbers.DistinctBy(x => x.Number))
                {
                    list.Add(from x in context.Set<WorkInfo>()
                             where x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number
                             select x);
                }

                returnList = returnList.Concat(await list.Aggregate((result, item) => result.Concat(item)).ToListAsync());
            }

            return returnList;
        }

        public async Task<bool> IsArchivedIswc(string preferredIswc) =>
            await context.Set<WorkInfo>().FromSqlInterpolated(
                $@"SELECT 1 as r FROM [ISWC].[WorkInfo] AS [w]
                INNER JOIN [ISWC].[ISWC] AS [i] ON [w].[IswcID] = [i].[IswcID]
                WHERE (((([w].[ArchivedIswc] = {preferredIswc}) AND ([w].[ArchivedIswc] IS NOT NULL AND {preferredIswc} IS NOT NULL))) AND ([i].[Status] = CAST(1 AS bit))) AND ([w].[Status] = CAST(1 AS bit))")
            .AnyAsync();

        public async Task<long> FindArchivedIswc(string archivedIswc)
        {
            var result = await context.RawSqlQuery($@"SELECT TOP 1 [w].IswcId
                   FROM [ISWC].[WorkInfo] AS [w]
                   WHERE (([w].[ArchivedIswc] = '{archivedIswc}')) and Status = 1", x => new { IswcId = x[0] });

            return (long)result.FirstOrDefault().IswcId;
        }
    }
}
