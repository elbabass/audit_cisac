using SpanishPoint.Azure.Iswc.Bdo.ComplexWork;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Options;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Search.Azure
{
    internal class AzureSearchService : ISearchService
    {
        private readonly ISearchClient<WorksOptions> worksSearchClient;

        public AzureSearchService(
            ISearchClient<WorksOptions> worksSearchClient)
        {
            this.worksSearchClient = worksSearchClient;
        }

        public async Task DeleteByWorkCode(long workCode)
        {
            var predicate = $"WorkID eq {workCode}";

            await worksSearchClient.Delete(predicate);
        }

        public async Task AddSubmission(Submission submission)
        {
            var works = GetWorks();
            await worksSearchClient.UploadAsync<ComplexWorksIndexModel>(works);
            
            IList<ComplexWorksIndexModel> GetWorks()
            {
                var worksToAdd = new List<ComplexWorksIndexModel>();
                var addWorkNum = new Bdo.Work.WorkNumber();

                if (submission.Model.AdditionalAgencyWorkNumbers.Any())
                    addWorkNum = submission.Model.AdditionalAgencyWorkNumbers.FirstOrDefault()?.WorkNumber;

                var work = !string.IsNullOrWhiteSpace(addWorkNum.Number) ?
                    submission.IswcModel.VerifiedSubmissions.FirstOrDefault(x => x.WorkNumber != null && addWorkNum.Number != null && x.WorkNumber.Number == addWorkNum.Number) :
                    submission.IswcModel.VerifiedSubmissions.FirstOrDefault(x => x.WorkNumber != null && submission.Model.WorkNumber != null && x.WorkNumber.Number == submission.Model.WorkNumber.Number);

                var additionalWorkNumbers = new List<WorkNumber>();
                {
                    if (submission.Model.AdditionalAgencyWorkNumbers.Any())
                        foreach (var aawn in submission.Model.AdditionalAgencyWorkNumbers)
                        {
                            if (!additionalWorkNumbers.Any(x => x.Number == aawn.WorkNumber.Number && x.TypeCode == aawn.WorkNumber.Type))
                                additionalWorkNumbers.Add(new WorkNumber { Number = aawn.WorkNumber.Number, TypeCode = aawn.WorkNumber.Type });
                        }
                }

                var workNumbers = new List<WorkNumber>
                {
                    new WorkNumber { Number = work.WorkNumber.Number, TypeCode = work.Agency },
                    new WorkNumber { Number = work.Iswc, TypeCode = "ISWC" }
                };

                if (additionalWorkNumbers.Any())
                    workNumbers.AddRange(additionalWorkNumbers);

                worksToAdd.Add(new ComplexWorksIndexModel
                {
                    GeneratedID = $"{work.WorkInfoID}",
                    WorkID = work.WorkInfoID,
                    MedleyType = null,
                    SocietyAccountNumber = null,
                    IsDeleted = false,
                    IsEligible = work.IswcEligible,
                    ExistsInDatabase = true,
                    IswcStatusID = Enum.TryParse(work.IswcStatus, out Bdo.Iswc.IswcStatus iswcStatus) ? (int)iswcStatus : 2,
                    WorkNames = work.IswcEligible ? work.Titles.Select(x => new Name { WorkName = x.Name, WorkNameType = (int)x.Type }) : null,
                    WorkNumbers = workNumbers,
                    WorkPerformers = work.Performers.Select(x => new WorkPerformer
                    {
                        PersonID = (long)x.PerformerID,
                        PersonFullName = x.FirstName + " " + x.LastName,
                        LastName = x.LastName,
                        RoleType = null
                    }),
                    WorkContributors = work.IswcEligible ? work.InterestedParties
                    .Where(x => x.CisacType == CisacInterestedPartyType.C || x.CisacType == CisacInterestedPartyType.MA || x.CisacType == CisacInterestedPartyType.TA)
                    .Select(x => new WorkContributor
                    {
                        PersonID = x.ContributorID != null ? (long)x.ContributorID : default,
                        IPIBaseNumber = x.IpBaseNumber,
                        IPICreatedDate = x.CreatedDate != null ? new DateTimeOffset(x.CreatedDate.Value) : default,
                        PersonFullName = x.Name,
                        LastName = x.LastName,
                        ContributorType = 2,
                        IPINumber = x.IPNameNumber,
                        RoleType = x.Type.ToString()
                    }) : null

                });

                return worksToAdd;
            }  
        }

        public async Task UpdateWorksIswcStatus(Data.DataModels.Iswc iswc)
        {
            var works = GetWorks();
            await worksSearchClient.MergeOrUploadAsync<ComplexWorksIndexModel>(works);

            IList<ComplexWorksIndexModel> GetWorks()
            {
                var worksToAdd = new List<ComplexWorksIndexModel>();
                var addWorkNum = new Bdo.Work.WorkNumber();

                foreach (var workinfo in iswc.WorkInfo.Where(x => x.Status))
                {
                    worksToAdd.Add(new ComplexWorksIndexModel
                    {
                        GeneratedID = $"{workinfo.WorkInfoId}",
                        WorkID = workinfo.WorkInfoId,
                        IswcStatusID = iswc.IswcStatusId
                    });
                }

                return worksToAdd;
            }
        }
    }
}
