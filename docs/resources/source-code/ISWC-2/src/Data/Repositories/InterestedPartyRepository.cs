using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IInterestedPartyRepository : IRepository<InterestedParty>
    {
        Task UpsertInterestedParty(InterestedParty interestedParty, InterestedParty existingInterestedParty, IList<Name> existingNames);
    }

    internal class InterestedPartyRepository : BaseRepository<InterestedParty>, IInterestedPartyRepository
    {
        public InterestedPartyRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }

        public async Task UpsertInterestedParty(InterestedParty interestedParty, InterestedParty existingInterestedParty, IList<Name> existingNames)
        {
            if (existingInterestedParty != null)
            {
                context.Agreement.RemoveRange(existingInterestedParty.Agreement);
                context.Status.RemoveRange(existingInterestedParty.Status);

                context.IpnameUsage.RemoveRange(existingInterestedParty.IpnameUsage);
                context.NameReference.RemoveRange(existingInterestedParty.NameReference);
                foreach (var name in existingInterestedParty.NameReference)
                {
                    if (existingNames.Any(x => x.IpnameNumber == name.IpnameNumberNavigation.IpnameNumber)
                        && name.IpnameNumberNavigation.NameReference.Count < 2)
                        context.Name.Remove(name.IpnameNumberNavigation);
                }

                context.InterestedParty.Remove(existingInterestedParty);
            }

            foreach (var usage in interestedParty.IpnameUsage)
            {
                var name = interestedParty?.NameReference.Where(i => i.IpnameNumber == usage.IpnameNumber)
                    .FirstOrDefault()?.IpnameNumberNavigation;

                if (existingNames.Any(x => x.IpnameNumber == name.IpnameNumber))
                {
                    usage.IpnameNumberNavigation = null;

                }
                else
                    usage.IpnameNumberNavigation = name;
            }

            if (existingNames.Any())
            {
                foreach (var nameReference in interestedParty.NameReference.Where(x => existingNames.Any(y => y.IpnameNumber == x.IpnameNumber)))
                {
                    nameReference.IpnameNumberNavigation = null;
                }

                context.Name.UpdateRange(existingNames);
            }

            await context.InterestedParty.AddAsync(interestedParty);

            await context.SaveChangesAsync();

        }
    }
}
