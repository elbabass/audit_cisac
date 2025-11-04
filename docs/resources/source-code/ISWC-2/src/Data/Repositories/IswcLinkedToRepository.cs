using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IIswcLinkedToRepository : IRepository<IswclinkedTo>
    {
        Task<IswcModel> GetLinkedToTree(IswcModel iswcModel);
    }

    internal class IswcLinkedToRepository : BaseRepository<IswclinkedTo>, IIswcLinkedToRepository
    {
        public IswcLinkedToRepository()
        {

        }
        public IswcLinkedToRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }

        public async Task<IswcModel> GetLinkedToTree(IswcModel iswcModel)
        {
            return await RecursiveLoad(iswcModel);

            async Task<IswcModel> RecursiveLoad(IswcModel parent)
            {
                var children = await context.IswclinkedTo.Include(x => x.Iswc).Where(x => x.LinkedToIswc == parent.Iswc && x.Status).ToListAsync();

                foreach (var childLinkedTo in children)
                {
                    var child = new IswcModel() { Iswc = childLinkedTo.Iswc.Iswc1 };

                    if (iswcModel.Iswc == child.Iswc)
                        return parent;

                    iswcModel.LinkedIswcTree.AddChild(child);
                    await RecursiveLoad(child);
                }
                return parent;
            }
        }
    }
}