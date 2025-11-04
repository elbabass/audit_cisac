using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi
{
    public class SuisaIpiService : IIpiService
    {
        private readonly HttpClient client;
        public SuisaIpiService(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("SuisaIpiClient");
        }

        public async IAsyncEnumerable<(InterestedParty interestedParty, DateTime watermark)> GetChangedIpsSinceDate(DateTime currentHighWaterMark, int batchSize)
        {
            var changedIps = await GetChangedIps(currentHighWaterMark, batchSize);
            if(changedIps != null)
            {
                foreach (var (baseNumber, watermark) in changedIps)
                {
                    yield return (await HydrateIp(baseNumber), watermark);
                }
            }
            else
            {
                yield return default;
            }
        }

        public async IAsyncEnumerable<InterestedParty> GetIps(List<string> baseNumbers)
        {
            foreach (var baseNumber in baseNumbers)
            {
                yield return await HydrateIp(baseNumber);
            }
        }

        private async Task<IEnumerable<(string baseNumber, DateTime watermark)>> GetChangedIps(DateTime waterMark, int batchSize)
        {
            var res = (await client.GetAsync($"getTxLogSummary?startTxTs={ waterMark:yyyyMMddHHmmss}&maxTx={ batchSize }")).EnsureSuccessStatusCode();
            var changedIps = DeserializeTransactionLogSummary(await res.Content.ReadAsStringAsync());

            if (!changedIps.Any())
                return default;

            return changedIps.GroupBy(x => x.baseNumber)
                .Select(y => y.OrderByDescending(w => w.watermark).FirstOrDefault())
                .OrderBy(x => x.watermark);
        }

        private async Task<InterestedParty> HydrateIp(string baseNumber)
        {
            var res = (await client.GetAsync($"getIPA?ipBaseNr={ baseNumber }&ExpandedCountry=false&CreationClasses=MW")).EnsureSuccessStatusCode();
            return DeserialiseIpaRecord(await res.Content.ReadAsStringAsync());
        }

        private InterestedParty DeserialiseIpaRecord(string ipaRecord)
        {
            var ip = new InterestedParty();

            foreach (var line in ipaRecord.Split("\r\n"))
            {
                if (line.Trim().Length > 2)
                {
                    var transactionType = line.Substring(0, 3);

                    switch (transactionType)
                    {
                        case "IPA":
                            var ipa = new IPA(line);
                            ip.IpbaseNumber = ipa.IpBaseNumber;
                            ip.AgencyId = ipa.RemittingSocietyCode;
                            break;

                        case "BDN":
                            var bdn = new BDN(line);
                            ip.BirthDate = bdn.GetDateOfBirth();
                            ip.DeathDate = bdn.GetDateOfDeath();
                            ip.Gender = bdn.Sex;
                            ip.BirthState = bdn.StateOfBirth;
                            ip.BirthPlace = bdn.PlaceOfBirth;
                            ip.AmendedDateTime = bdn.AmendedDate ?? default;
                            ip.Type = bdn.InterestedPartyType.ToString();
                            break;

                        case "STN":
                            var status = new STN(line);
                            ip.Status.Add(new Status
                            {
                                StatusCode = (int)status.StatusCode,
                                FromDate = status.ValidFrom ?? default,
                                ToDate = status.ValidTo ?? default,
                                ForwardingBaseNumber = status.IPBaseNumber,
                                IpbaseNumber = ip.IpbaseNumber
                            });
                            break;

                        case "NUN":
                            var nun = new NUN(line);
                            ip.IpnameUsage.Add(new IpnameUsage
                            {
                                IpbaseNumber = ip.IpbaseNumber,
                                IpnameNumber = nun.IpNameNumber,
                                CreationClass = nun.CreationCode,
                                Role = nun.RoleCode
                            });
                            break;

                        case "MAN":
                            var man = new MAN(line);
                            ip.Agreement.Add(new Agreement
                            {
                                AgencyId = man.SociteyCode,
                                CreationClass = man.ClassCode,
                                Role = man.RoleCode,
                                EconomicRights = man.RightCode,
                                FromDate = man.ValidFromDateTime,
                                ToDate = man.ValidToDateTime,
                                SignedDate = man.DateOfSignature,
                                SharePercentage = man.MembershipShare,
                                AmendedDateTime = man.AmendmentDateTime,
                                IpbaseNumber = ip.IpbaseNumber
                            });
                            break;

                        case "NCN":
                            var ncn = new NCN(line);
                            ip.NameReference.Add(
                                new NameReference
                                {
                                    IpbaseNumber = ip.IpbaseNumber,
                                    IpnameNumber = ncn.IPNameNumber,
                                    AmendedDateTime = ncn.AmendmentDate,
                                    IpnameNumberNavigation = new Name
                                    {
                                        IpnameNumber = ncn.IPNameNumber,
                                        AmendedDateTime = ncn.AmendmentDate,
                                        FirstName = ncn.FirstName,
                                        LastName = ncn.Name,
                                        CreatedDate = ncn.CreationDate,
                                        TypeCode = ncn.NameType.ToString(),
                                        AgencyId = ip.AgencyId,
                                        IpnameUsage = null
                                    }
                                });
                            break;

                        case "MCN":
                            var mcn = new MCN(line);
                            ip.NameReference.Add(new NameReference
                            {
                                IpnameNumber = mcn.IpNameNumber,
                                IpbaseNumber = mcn.IpBaseNumber,
                                AmendedDateTime = mcn.AmendmentDateTime,
                                IpnameNumberNavigation = new Name
                                {
                                    IpnameNumber = mcn.IpNameNumber,
                                    AmendedDateTime = mcn.AmendmentDateTime,
                                    LastName = mcn.Name,
                                    CreatedDate = mcn.CreationDateTime,
                                    TypeCode = mcn.NameType,
                                    AgencyId = ip.AgencyId,
                                    IpnameUsage = null
                                }
                            });
                            break;

                        case "ONN":
                            var onn = new ONN(line);
                            ip.NameReference.Add(
                                new NameReference
                                {
                                    IpbaseNumber = ip.IpbaseNumber,
                                    IpnameNumber = onn.IpNameNumber,
                                    AmendedDateTime = onn.AmendmentDateTime,
                                    IpnameNumberNavigation = new Name
                                    {
                                        IpnameNumber = onn.IpNameNumber,
                                        AmendedDateTime = onn.AmendmentDateTime,
                                        FirstName = onn.FirstName,
                                        LastName = onn.Name,
                                        CreatedDate = onn.CreationDateTime,
                                        TypeCode = onn.NameType.ToString(),
                                        AgencyId = ip.AgencyId,
                                        ForwardingNameNumber = onn.IpNameNumberRef,
                                        IpnameUsage = null
                                    }
                                });
                            break;

                        case "INN":
                            var inn = new INN(line);
                            ip.IpnameUsage.Add(new IpnameUsage
                            {
                                IpbaseNumber = ip.IpbaseNumber,
                                IpnameNumber = inn.IpNameNumber,
                                CreationClass = inn.CreationCode,
                                Role = inn.RoleCode
                            });
                            break;

                        case "IMN":
                            var imn = new IMN(line);
                            ip.IpnameUsage.Add(new IpnameUsage
                            {
                                IpbaseNumber = ip.IpbaseNumber,
                                IpnameNumber = imn.IpNameNumber,
                                CreationClass = imn.CreationCode,
                                Role = imn.RoleCode
                            });
                            break;

                    }
                }
            }

            return ip;
        }

        public static IEnumerable<(string baseNumber, DateTime watermark)> DeserializeTransactionLogSummary(string rawLogSummary)
        {
            foreach (var line in rawLogSummary.Split("\r\n"))
            {
                if (line.Trim().Length > 2)
                {
                    var transactionType = line.Substring(0, 3);
                    switch (transactionType)
                    {
                        case "TLS":
                            var ipa = new TLS(line);
                            yield return (ipa.IpBaseNumber, ipa.TransactionDate);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
