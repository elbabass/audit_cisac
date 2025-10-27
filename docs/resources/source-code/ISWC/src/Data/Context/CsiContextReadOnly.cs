using Microsoft.EntityFrameworkCore;

namespace SpanishPoint.Azure.Iswc.Data.DataModels.Context
{
    public class CsiContextReadOnly : CsiBaseContext
    {
        public CsiContextReadOnly(DbContextOptions options) : base(options)
        {
        }
    }
}
