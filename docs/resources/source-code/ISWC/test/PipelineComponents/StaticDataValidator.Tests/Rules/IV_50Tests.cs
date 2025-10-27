using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_50
    /// </summary>
    public class IV_50Tests : TestBase
    {
        /// <summary>
        /// Check E IPs are removed
        /// </summary>
        [Fact]
        public async void IV_50_E_IPs_Removed()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_50>();

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() { Type = InterestedPartyType.C, IPNameNumber=1244325 },
                new InterestedPartyModel() { Type = InterestedPartyType.E }
            };
            var response = await test.Object.IsValid(submission);

            Assert.Single(response.Submission.Model.InterestedParties);
            Assert.Equal(nameof(IV_50), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check AM IPs are removed
        /// </summary>
        [Fact]
        public async void IV_50_AM_IPs_Removed()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_50>();

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() { Type = InterestedPartyType.AM },
                new InterestedPartyModel() { Type = InterestedPartyType.C, IPNameNumber=1244325 }
            };
            var response = await test.Object.IsValid(submission);

            Assert.Single(response.Submission.Model.InterestedParties);
            Assert.Equal(nameof(IV_50), test.Object.Identifier);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check Non AM and E IPs are not removed
        /// </summary>
        [Fact]
        public async void IV_50_Non_AM_E_IPs_Not_Removed()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_50>();

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() { Type = InterestedPartyType.C },
                new InterestedPartyModel() { Type = InterestedPartyType.C, IPNameNumber=1244325 }
            };
            var response = await test.Object.IsValid(submission);

            Assert.Equal(2, response.Submission.Model.InterestedParties.Count);
            Assert.Equal(nameof(IV_50), test.Object.Identifier);
            Assert.True(response.IsValid);
        }


        /// <summary>
        /// Check AM and E IPs with IP Name Numbers are not removed
        /// </summary>
        [Fact]
        public async void IV_50_Non_AM_E_IPs_With_IpNameNumbers_Not_Removed()
        {
            var submission = new Submission() { Model = new SubmissionModel() };

            var test = new Mock<IV_50>();

            submission.Model.InterestedParties = new List<InterestedPartyModel>() {
                new InterestedPartyModel() { Type = InterestedPartyType.A, IPNameNumber = 432423 },
                new InterestedPartyModel() { Type = InterestedPartyType.E, IPNameNumber=1244325 }
            };
            var response = await test.Object.IsValid(submission);

            Assert.Equal(2, response.Submission.Model.InterestedParties.Count);
            Assert.Equal(nameof(IV_50), test.Object.Identifier);
            Assert.True(response.IsValid);
        }
    }
}
