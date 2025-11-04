using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.Iswc
{
    public class ISWCMetadataModel
    {
        public ISWCMetadataModel()
        {
            Titles = new List<Title>();
            InterestedParties = new List<InterestedPartyModel>();
        }
        public ICollection<InterestedPartyModel> InterestedParties { get; set; }
        public ICollection<Title> Titles { get; set; }

    }
}
