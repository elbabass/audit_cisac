using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkSuccessfullyTests_OverallParentPerformance_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchclient;
        public IISWC_MergeClient mergeclient;
        public int[] testCases;
        public HttpClient httpClient;
        public List<List<Submission>> submissionsList;
        public List<double> averageSubmissionTiming;
        public List<double> averageMergeTiming;
        public List<List<double>> averageUpdateTiming;
        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchclient = new ISWC_SearchClient(httpClient);
            mergeclient = new ISWC_MergeClient(httpClient);
            //Chain Lengths to test against
            testCases = new int[] { 1, 2, 3 };
            submissionsList = new List<List<Submission>>();
            averageSubmissionTiming = new List<double>();
            averageMergeTiming = new List<double>();
            averageUpdateTiming = new List<List<double>>();
            foreach (var num in testCases)
            {
                submissionsList.Add(await GenerateISWCChainAlternate(num));
            }
            await UpdateGeneratedISWCs();
        }
        async Task<List<Submission>> GenerateISWCChainAlternate(int chainLength)
        {
            Stopwatch timer = new Stopwatch();
            List<Submission> currentSubs = new List<Submission>();
            /// Generate Submissions
            for (int i = 0; i < chainLength; i++)
            {
                Submission sub = Submissions.EligibleSubmissionIMRO;

                //Time the submission
                timer.Start();
                sub.Iswc = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
                timer.Stop();
                averageSubmissionTiming.Add(timer.Elapsed.TotalSeconds);
                timer.Reset();
                await TestBase.WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
                currentSubs.Add(sub);
            }
            /// Merge Submissions into Chain
            for (int i = 0; i < chainLength - 1; i++)
            {
                var body = new Body
                {
                    Iswcs = new string[] { currentSubs[i].Iswc }
                };
                timer.Start();
                await mergeclient.MergeISWCMetadataAsync(currentSubs[i + 1].Iswc, currentSubs[i].Agency, body);
                timer.Stop();
                averageMergeTiming.Add(timer.Elapsed.TotalSeconds);
                timer.Reset();
                //Check merged ISWC is present in linked ISWCs
                var searchRes = await searchclient.SearchByISWCAsync(currentSubs[i + 1].Iswc);
                Assert.True(searchRes.LinkedISWC.Where(x => x == currentSubs[i].Iswc).Count() > 0);
                //Check merge target ISWC is current overall parent
                searchRes = await searchclient.SearchByISWCAsync(currentSubs[0].Iswc);
                Assert.True(searchRes.Works.FirstOrDefault().OverallParentISWC == currentSubs[i + 1].Iswc);
            }
            return currentSubs;
        }
        async Task UpdateGeneratedISWCs()
        {
            Stopwatch timer = new Stopwatch();
            foreach (var submission in submissionsList)
            {
                for (int i = 0; i < 10; i++)
                {
                    var localSub = submission[0];
                    localSub.Iswc = null;
                    timer.Start();
                    await submissionClient.AddSubmissionAsync(localSub);
                    timer.Stop();
                    while (averageUpdateTiming.Count < submission.Count)
                    {
                        averageUpdateTiming.Add(new List<double>());
                    }
                    averageUpdateTiming[submission.Count - 1].Add(timer.Elapsed.TotalSeconds);
                    timer.Reset();
                }
            }
        }
        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
    public class UpdateExistingWorkSuccessfullyTests_OverallParentPerformance : TestBase, IClassFixture<UpdateExistingWorkSuccessfullyTests_OverallParentPerformance_Fixture>
    {
        private readonly List<double> averageSubmissionTiming;
        private readonly List<double> averageMergeTiming;
        private readonly List<List<double>> averageUpdateTiming;
        public UpdateExistingWorkSuccessfullyTests_OverallParentPerformance(UpdateExistingWorkSuccessfullyTests_OverallParentPerformance_Fixture fixture)
        {
            averageSubmissionTiming = fixture.averageSubmissionTiming;
            averageMergeTiming = fixture.averageMergeTiming;
            averageUpdateTiming = fixture.averageUpdateTiming;
        }
        /// <summary>
        /// Test that the timings for the submissions recorded are acceptable
        /// Acceptable: Around 3 seconds per submission with no chain of ISWCs, with scope for a small increase scaling with ISWC chain length
        /// Small: TBD
        /// </summary>
        [Fact(Skip = "TODO: Move to performance test.")]
        public void UpdateExistingWorkSuccessfullyTests_OverallParentPerformance_01()
        {
            Assert.True(averageSubmissionTiming.Count > 0);
            Assert.InRange(averageSubmissionTiming.Sum(x => x) / averageSubmissionTiming.Count, 0, 5);
        }
        /// <summary>
        /// Test that the timings for the merges recorded are acceptable
        /// </summary>
        [Fact(Skip = "TODO: Move to performance test.")]
        public void UpdateExistingWorkSuccessfullyTests_OverallParentPerformance_02()
        {
            Assert.True(averageMergeTiming.Count > 0);
            Assert.InRange(averageMergeTiming.Sum(x => x) / averageMergeTiming.Count, 0, 5);
        }
        /// <summary>
        /// Test that the timings for the updates recorded are acceptable
        /// </summary>
        [Fact(Skip = "TODO: Move to performance test.")]
        public void UpdateExistingWorkSuccessfullyTests_OverallParentPerformance_03()
        {
            Assert.True(averageUpdateTiming.Count > 0);
            List<double> averages = new List<double>();
            foreach (var list in averageUpdateTiming)
            {
                averages.Add(list.Sum(x => x) / list.Count());
            }
            Assert.All(averages, item => Assert.InRange(item, 0, 5));
        }
    }
}