using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.IswcService
{
    public interface IIswcService
    {
        Task<string> GetNextIswc();
    }
}
