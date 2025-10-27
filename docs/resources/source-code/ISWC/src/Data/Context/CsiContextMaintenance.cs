using Microsoft.EntityFrameworkCore;

namespace SpanishPoint.Azure.Iswc.Data.DataModels.Context
{
    public class CsiContextMaintenance : CsiBaseContext
    {
        public CsiContextMaintenance(DbContextOptions options) : base(options)
        {
        }
    }
}
