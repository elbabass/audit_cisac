using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Tests.Rules
{
    /// <summary>
    /// Tests for rule PV_10
    /// </summary>
    public class PV_12Tests : TestBase
    {
        /// <summary>
        /// Check if a merge exists so demerge transaction can be carried out.
        /// </summary>
        [Fact]
        public async void PV_12_Valid()
        {
            var sub = new Submission()
            {
                Model = new SubmissionModel
                {
                    WorkNumber = new WorkNumber { Number = "WORK0001", Type = "128" },
                    PreferredIswc = "T1000001232"
                }
            };

            var linkedToManger = new Mock<ILinkedToManager>();

            linkedToManger.Setup(w => w.MergeExists(sub.Model.PreferredIswc, sub.Model.WorkNumber))
             .ReturnsAsync(true);

            var test = new Mock<PV_12>(GetMessagingManagerMock(ErrorCode._172), linkedToManger.Object);

            var response = await test.Object.IsValid(sub);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(PV_12), test.Object.Identifier);
        }

        /// <summary>
        /// Check if a merge doesn't exist and causes demerge to fail.
        /// </summary>
        [Fact]
        public async void PV_12_InValid()
        {
            var sub = new Submission()
            {
                Model = new SubmissionModel
                {
                    WorkNumber = new WorkNumber { Number = "WORK0001", Type = "128" },
                    PreferredIswc = "T1000001232"
                }
            };

            var linkedToManger = new Mock<ILinkedToManager>();

            linkedToManger.Setup(w => w.MergeExists(sub.Model.PreferredIswc, sub.Model.WorkNumber))
             .ReturnsAsync(false);

            var test = new Mock<PV_12>(GetMessagingManagerMock(ErrorCode._172), linkedToManger.Object);

            var response = await test.Object.IsValid(sub);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(PV_12), test.Object.Identifier);
            Assert.Equal(ErrorCode._172, response.Submission.Rejection.Code);
        }
    }
}
