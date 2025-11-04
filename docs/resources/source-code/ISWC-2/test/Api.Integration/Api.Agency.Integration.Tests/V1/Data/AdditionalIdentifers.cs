using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data
{
    public static class AdditionalIdentifersData
    {
        public static AdditionalIdentifiers AI_JASRAC => new AdditionalIdentifiers()
        {
            Isrcs = new List<string> { TestBase.CreateNewWorkCode() },
            PublisherIdentifiers = new List<PublisherIdentifiers> {
            new PublisherIdentifiers{
                NameNumber = 589330613,
                WorkCode = new List<string> {TestBase.CreateNewWorkCode()}
            }
                }
        };

        public static AdditionalIdentifiers AI_SONY => new AdditionalIdentifiers()
        {
            Isrcs = new List<string> { TestBase.CreateNewWorkCode() },
            PublisherIdentifiers = new List<PublisherIdentifiers> {
            new PublisherIdentifiers{
                NameNumber = 269137346,
                WorkCode = new List<string> { TestBase.CreateNewWorkCode() }
            }
            }
        };

        public static AdditionalIdentifiers AI_NoSociety => new AdditionalIdentifiers()
        {
            Isrcs = new List<string> { TestBase.CreateNewWorkCode() },
            PublisherIdentifiers = new List<PublisherIdentifiers> {
            new PublisherIdentifiers{

                NameNumber = 269021863,
                WorkCode = new List<string> {TestBase.CreateNewWorkCode()}
            }
            }
        };

        public static AdditionalIdentifiers AI_WARNER => new AdditionalIdentifiers()
        {
            Isrcs = new List<string> { TestBase.CreateNewWorkCode() },
            PublisherIdentifiers = new List<PublisherIdentifiers> {
            new PublisherIdentifiers{
                NameNumber = 76448253,
                WorkCode = new List<string> { TestBase.CreateNewWorkCode() }
            }
            }
        };

        public static AdditionalIdentifiers AI_PUB_MultipleCodes(int numCodes, int pubNameNumber, string submitterCode)
        {
            AdditionalIdentifiers additionalIdentifiers = new AdditionalIdentifiers()
            {
                Isrcs = new List<string> { TestBase.CreateNewWorkCode() },
                PublisherIdentifiers = new List<PublisherIdentifiers>
                {
                    new PublisherIdentifiers
                    {
                        NameNumber = pubNameNumber,
                        WorkCode = new List<string>(),
                        SubmitterCode = submitterCode
                    }
                }
            };

            for (int i = 0; i < numCodes; i++)
            {
                additionalIdentifiers.PublisherIdentifiers.FirstOrDefault().WorkCode.Add(TestBase.CreateNewWorkCode());
            }

            return additionalIdentifiers;
        }
    }
}
