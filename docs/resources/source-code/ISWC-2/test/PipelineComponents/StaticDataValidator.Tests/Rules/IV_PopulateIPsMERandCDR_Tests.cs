using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
	/// <summary>
	/// IV/56 tests
	/// </summary>
	public class IV_PopulateIPsMERandCDR_Tests
	{
        /// <summary>
        /// Check if workinfo is titles and interested parties are updated if workinfo is present
        /// </summary>
        [Fact]
        public async Task IV_WorkInfo_and_interestedParties_Exist()
        {
            var submission = new Submission(){ Model = new SubmissionModel() };
            var workManagerMock = new Mock<IWorkManager>();
            var workNumber = new WorkNumber() { Type = "10", Number = "00000032" };

            workManagerMock.Setup(w => w.FindVerifiedAsync("1111", It.IsAny<string>())).ReturnsAsync(default(VerifiedSubmissionModel));
            workManagerMock.Setup(w => w.FindVerifiedAsync(It.IsAny<WorkNumber>())).ReturnsAsync(new VerifiedSubmissionModel() { WorkNumber = workNumber, Iswc = "1111" });

            var test = new Mock<IV_PopulateIPsMERandCDR>(workManagerMock.Object);

            var response = await test.Object.IsValid(submission);

            Assert.Equal(nameof(IV_PopulateIPsMERandCDR), test.Object.Identifier);
            Assert.True(response.IsValid);
        }
    }
}
