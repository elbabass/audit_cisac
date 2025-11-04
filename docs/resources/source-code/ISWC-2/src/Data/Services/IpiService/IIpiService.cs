using SpanishPoint.Azure.Iswc.Data.DataModels;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService
{
    public interface IIpiService
    {
        IAsyncEnumerable<(InterestedParty interestedParty, DateTime watermark)> GetChangedIpsSinceDate(DateTime currentHighWaterMark, int batchSize);
        IAsyncEnumerable<InterestedParty> GetIps(List<string> baseNumbers);
    }
}
