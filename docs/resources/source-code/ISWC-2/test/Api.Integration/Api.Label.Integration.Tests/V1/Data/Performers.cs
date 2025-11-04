using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data
{
    public class Performers
    {
        public static List<Performer> Performers_CISAC => new List<Performer>()
        {
            new Performer
            {
                Isni = "0000000440067795",
                FirstName = "Andrew",
                LastName = "Hozier Byrne",
                Designation = PerformerDesignation.Main_Artist
            },
            new Performer
            {
                Isni = "000000046361996X",
                LastName = "Bedouine",
                Designation = PerformerDesignation.Main_Artist
            }
        };
    }
}
