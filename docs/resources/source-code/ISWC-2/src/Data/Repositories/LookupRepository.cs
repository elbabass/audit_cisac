using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Data.Extensions;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LookupData = SpanishPoint.Azure.Iswc.Bdo.Work.LookupData;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
	public interface ILookupRepository
    {
        Task<IEnumerable<LookupData>> GetLookups();
    }

    internal class LookupRepository : ILookupRepository
    {
        private readonly CsiContext context;

        public LookupRepository(CsiContext context)
        {
            this.context = context;
        }

        public Task<IEnumerable<LookupData>> GetLookups()
        {
            var lookupTables = new[] {
                typeof(Agency),
                typeof(Instrumentation),
                typeof(TitleType)
            };

            var submissionSource = context.SubmissionSource.Select(x => x.Code.Trim()).ToList();
            IList<LookupData> lookups = new List<LookupData>();

            foreach (var table in lookupTables)
            {
                IList<object> rows = new List<object>();
                
                foreach (dynamic row in context.Set(table))
                {
                    if (HasProperty(row, "Family"))
                        rows.Add(new { row.Code, row.Name, row.Family });
                    else if (HasProperty(row, "Name"))
					{
                        PropertyInfo propertyInfo = row.GetType().GetProperty("AgencyId");
                        if(submissionSource.Contains(propertyInfo.GetValue(row).ToString()))
                            rows.Add(new { row.AgencyId, row.Name });
                    }  
                    else if (HasProperty(row, "Code"))
                        rows.Add(new { row.Code, row.Description });
                };

				lookups.Add(new LookupData(table.Name, rows));
            }
            return Task.FromResult(lookups.AsEnumerable());

            static bool HasProperty(dynamic obj, string name)
            {
                var objType = obj.GetType();

                if (objType == typeof(ExpandoObject))
                    return ((IDictionary<string, object>)obj).ContainsKey(name);

                return objType.GetProperty(name) != null;
            }
        }
    }
}
