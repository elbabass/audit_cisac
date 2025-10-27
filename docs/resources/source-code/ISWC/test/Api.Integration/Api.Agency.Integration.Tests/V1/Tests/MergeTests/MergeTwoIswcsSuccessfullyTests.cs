using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using xRetry;
using Xunit;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.MergeTests
{
    public class MergeTwoIswcsSuccessfullyTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchclient;
        public IISWC_MergeClient mergeclient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchclient = new ISWC_SearchClient(httpClient);
            mergeclient = new ISWC_MergeClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class MergeTwoIswcsSuccessfullyTests : TestBase, IClassFixture<MergeTwoIswcsSuccessfullyTests_Fixture>
    {
        private readonly IISWC_MergeClient mergeclient;
        private readonly IISWC_SearchClient searchclient;
        private readonly IISWC_SubmissionClient submissionClient;
        private readonly HttpClient httpClient;


        public MergeTwoIswcsSuccessfullyTests(MergeTwoIswcsSuccessfullyTests_Fixture fixture)
        {
            mergeclient = fixture.mergeclient;
            searchclient = fixture.searchclient;
            submissionClient = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }
        /// <summary>
        /// Submitter is ISWC eligible
        /// Preferred ISWCs exist
        /// Merge one ISWC into parent
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_01()
        {
            var sub = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            var childOt = CreateNewTitle();
            sub.OriginalTitle = childOt;
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);

            var res = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Collection(res.LinkedISWC,
                elem1 => Assert.Equal(body.Iswcs.ElementAt(0), elem1));
            Assert.Equal(childOt, res.OriginalTitle);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Agency Work Numbers exist
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_02()
        {
            var sub = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                AgencyWorks = new WorkNumber[]
                {
                    new WorkNumber
                    {
                        AgencyCode=sub.Agency,
                        AgencyWorkCode=sub.Workcode
                    }
                }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);
            var res = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Contains(iswcTwo, res.LinkedISWC);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Preferred ISWCs exist
        /// Merge mutliple ISWC's into parent
        /// </summary>
        [RetryFact]
        public async void MergeTwoIswcsSuccessfullyTests_03()
        {
            var sub = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var iswcThree = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo, iswcThree }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);
            var res = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Contains(iswcTwo, res.LinkedISWC);
            Assert.Contains(iswcThree, res.LinkedISWC);
        }

        /// Merge is submitted by parent agency under IncludeAgenciesInEligibilityCheck.
        /// IP's belong to a child agency.
        /// </summary>
        [SkippableFact]
        public async void MergeTwoIswcsSuccessfullyTests_04()
        {
            Skip.IfNot((await GetValidationParameterValue("IncludeAgenciesInEligibilityCheck")).Contains("030"), "AMAR not included in IncludeAgenciesInEligibilityCheck.");
            var sub = Submissions.EligibleSubmissionECAD;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);
            var res = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Contains(iswcTwo, res.LinkedISWC);
        }

        /// <summary>
        /// Merges IswcTwo into IswcOne and then updates the child work.
        /// Update is applied to the parent ISWC.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_05()
        {
            var sub = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);


            sub.OriginalTitle += "123";
            sub.InterestedParties.Add(InterestedParties.IP_BMI[0]);
            var res = await submissionClient.UpdateSubmissionAsync(iswcOne, sub);
            await WaitForUpdate(sub.Agency, sub.Workcode, sub.OriginalTitle, httpClient);

            var searchRes = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Equal(sub.OriginalTitle, searchRes.OriginalTitle);
            Assert.Contains(searchRes.Works, x => x.Workcode.Equals(sub.Workcode));
            Assert.Contains(searchRes.InterestedParties, x => x.NameNumber.Equals(InterestedParties.IP_BMI[0].NameNumber));
        }

        /// <summary>
        /// Merged work contains more IP's than parent.
        /// Consolidated ISWC info for parent will only show the IP's from the parent.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_06()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionASCAP;
            childWork.InterestedParties = InterestedParties.IP_ASCAP;
            childWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var iswcTwo = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, parentWork.Agency, body);
            await Task.Delay(2000);

            var res = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Equal(2, res.InterestedParties.Count);
        }

        /// <summary>
        /// Merged work contains more IP's than parent.
        /// Consolidated ISWC info for parent will only show the IP's from the parent until child is updated.
        /// When merged work is updated then the IP's are not excluded.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_07()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionASCAP;
            childWork.InterestedParties = InterestedParties.IP_ASCAP;
            childWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var iswcTwo = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, parentWork.Agency, body);
            await Task.Delay(2000);

            childWork.OriginalTitle = $"{parentWork.OriginalTitle} a";
            childWork.PreferredIswc = iswcOne;
            await submissionClient.UpdateSubmissionAsync(iswcOne, childWork);
            await WaitForUpdate(childWork.Agency, childWork.Workcode, childWork.OriginalTitle, httpClient);

            var res = await searchclient.SearchByISWCAsync(iswcOne);

            Assert.Equal(InterestedParties.IP_ASCAP.Count + 1, res.InterestedParties.Count);
        }

        /// <summary>
        /// ParentWork agency is elligible on parent work only.
        /// ChildWork agency is eligible on child work only.
        /// ChildWork agency has an inelligible submission on parent.
        /// Merge as ChildWork agency is eligible.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_08()
        {
            var parentWork = Submissions.EligibleSubmissionBMI;
            var parentIswc = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(parentWork.Agency, parentWork.Workcode, httpClient);
            parentWork.Agency = Submissions.EligibleSubmissionASCAP.Agency;
            parentWork.Sourcedb = Submissions.EligibleSubmissionASCAP.Sourcedb;
            parentWork.Workcode = CreateNewWorkCode();
            await submissionClient.AddSubmissionAsync(parentWork);
            await WaitForSubmission(parentWork.Agency, parentWork.Workcode, httpClient);
            var childWork = Submissions.EligibleSubmissionASCAP;
            var childIswc = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentIswc, childWork.Agency, body);
            await Task.Delay(2000);

            var res = await searchclient.SearchByISWCAsync(parentIswc);

            Assert.Collection(res.LinkedISWC,
                elem1 => Assert.Equal(body.Iswcs.ElementAt(0), elem1));
        }

        /// <summary>
        /// ParentWork agency is eligible on ISWC One.
        /// ChildWork agency is eligible on ISWC Two.
        /// Each agency only has a submission on one ISWC.
        /// Merge as ParentWork agency is eligible.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_09()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            var parentIswc = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionBMI;
            var childIswc = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentIswc, childWork.Agency, body);
            await Task.Delay(2000);

            var res = await searchclient.SearchByISWCAsync(parentIswc);

            Assert.Collection(res.LinkedISWC,
                elem1 => Assert.Equal(body.Iswcs.ElementAt(0), elem1));
        }

        /// <summary>
        /// Bug 6747 - Merging multiple iswc's, demerging and remerging creates duplicate works on children
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_10()
        {
            var parentSubmission = Submissions.EligibleSubmissionBMI;
            var mergeSubmissionOne = Submissions.EligibleSubmissionBMI;
            var mergeSubmissionTwo = Submissions.EligibleSubmissionBMI;
            parentSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(parentSubmission))
                .VerifiedSubmission.Iswc.ToString();
            mergeSubmissionOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(mergeSubmissionOne))
                .VerifiedSubmission.Iswc.ToString();
            mergeSubmissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(mergeSubmissionTwo))
                .VerifiedSubmission.Iswc.ToString();
            var body = new Body
            {
                Iswcs = new string[] { mergeSubmissionOne.PreferredIswc, mergeSubmissionTwo.PreferredIswc }
            };
            await WaitForSubmission(mergeSubmissionOne.Agency, mergeSubmissionOne.Workcode, httpClient);
            await WaitForSubmission(parentSubmission.Agency, parentSubmission.Workcode, httpClient);

            await mergeclient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, body);
            await Task.Delay(2000);

            await mergeclient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, mergeSubmissionOne.Workcode);
            await mergeclient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, mergeSubmissionTwo.Workcode);
            await Task.Delay(2000);

            await mergeclient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, body);
            await Task.Delay(2000);

            var parent = await searchclient.SearchByISWCAsync(parentSubmission.PreferredIswc);
            var childOne = await searchclient.SearchByISWCAsync(mergeSubmissionOne.PreferredIswc);
            var childTwo = await searchclient.SearchByISWCAsync(mergeSubmissionTwo.PreferredIswc);

            Assert.Equal(1, childOne.Works.Count);
            Assert.Equal(1, childTwo.Works.Count);
            Assert.Equal(3, parent.Works.Count);
        }

        /// <summary>
        /// Merges child into parent and then updates the parent work.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_11()
        {
            var parentSub = Submissions.EligibleSubmissionASCAP;
            parentSub.PreferredIswc = (await submissionClient.AddSubmissionAsync(parentSub)).VerifiedSubmission.Iswc.ToString();
            var childSub = Submissions.EligibleSubmissionASCAP;
            childSub.PreferredIswc = (await submissionClient.AddSubmissionAsync(childSub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childSub.Agency, childSub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { childSub.PreferredIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentSub.PreferredIswc, parentSub.Agency, body);
            await Task.Delay(2000);


            parentSub.OriginalTitle += "123";
            parentSub.InterestedParties.Add(InterestedParties.IP_BMI[0]);
            var res = await submissionClient.UpdateSubmissionAsync(parentSub.PreferredIswc, parentSub);
            await WaitForUpdate(parentSub.Agency, parentSub.Workcode, parentSub.OriginalTitle, httpClient);

            var searchRes = await searchclient.SearchByISWCAsync(parentSub.PreferredIswc);

            Assert.Equal(parentSub.OriginalTitle, searchRes.OriginalTitle);
            Assert.Equal(3, searchRes.InterestedParties.Count);
        }

        /// <summary>
        /// Overall Parent - Assemble chain of ISWCs and verify overall parent returned is correct
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsSuccessfullyTests_12()
        {
            int chainLength = 3;
            List<Submission> currentSubs = new List<Submission>();
            /// Generate Submissions
            for (int i = 0; i < chainLength; i++)
            {
                Submission sub = Submissions.EligibleSubmissionIMRO;

                //Time the submission
                sub.Iswc = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
                await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
                currentSubs.Add(sub);
            }
            /// Merge Submissions into Chain
            for (int i = 0; i < chainLength - 1; i++)
            {
                var body = new Body
                {
                    Iswcs = new string[] { currentSubs[i].Iswc }
                };
                await mergeclient.MergeISWCMetadataAsync(currentSubs[i + 1].Iswc, currentSubs[i].Agency, body);
                //Check merged ISWC is present in linked ISWCs
                var searchRes = await searchclient.SearchByISWCAsync(currentSubs[i + 1].Iswc);
                Assert.True(searchRes.LinkedISWC.Where(x => x == currentSubs[i].Iswc).Count() > 0);
                //Check merge target ISWC is current overall parent
                searchRes = await searchclient.SearchByISWCAsync(currentSubs[0].Iswc);
                Assert.True(searchRes.Works.FirstOrDefault().OverallParentISWC == currentSubs[i + 1].Iswc);
            }
        }
    }
}
