using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Api.ReadyCheck.Tests
{
    public class ReadyCheck_Tests
    {
        [Fact]
        public async void CheckReadiness()
        {
            var httpClient = await TestBase.GetAgencyClient();
            var subClient = new ISWC_SubmissionClient(httpClient);
            var mergeClient = new ISWC_MergeClient(httpClient);

            var subOne = Submissions.EligibleSubmissionASCAP;
            var subTwo = Submissions.EligibleSubmissionASCAP;

            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool success = false;
            while (timer.Elapsed.TotalSeconds < 180 && !success)
            {
                try
                {
                    var iswc = (await subClient.AddSubmissionAsync(subOne)).VerifiedSubmission.Iswc.ToString();
                    success = true;
                    subOne.PreferredIswc = iswc;
                    await Task.Delay(3000);
                    break;
                }
                catch
                {
                    await Task.Delay(3000);
                }
            }
            if (!success)
            {
                throw new XunitException("Could not add submission");
            }

            timer = new Stopwatch();
            timer.Start();
            success = false;
            subOne.OriginalTitle = TestBase.CreateNewTitle();
            while (timer.Elapsed.TotalSeconds < 180 && !success)
            {
                try
                {
                    await subClient.UpdateSubmissionAsync(subOne.PreferredIswc, subOne);
                    success = true;
                    await Task.Delay(3000);
                    break;
                }
                catch
                {
                    await Task.Delay(3000);
                }
            }
            if (!success)
            {
                throw new XunitException("Could not update submission");
            }

            subTwo.PreferredIswc = (await subClient.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            timer = new Stopwatch();
            timer.Start();
            success = false;
            while (timer.Elapsed.TotalSeconds < 180 && !success)
            {
                try
                {
                    var body = new Body
                    {
                        Iswcs = new string[] { subTwo.PreferredIswc }
                    };
                    
                    await mergeClient.MergeISWCMetadataAsync(subOne.PreferredIswc, subOne.Agency, body);
                    success = true;
                    break;
                }
                catch (Exception e)
                {
                    await Task.Delay(3000);
                }
            }
            if (!success)
            {
                throw new XunitException("Could not merge submissions");
            }

        }
    }
}
