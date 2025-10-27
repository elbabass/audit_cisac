using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_51
    /// </summary>
    public class IV_51Tests : TestBase
    {
        /// <summary>
        /// Check transaction passes if
        /// request source is from portal 
        /// </summary>
        [Fact]
        public async void IV_51_Valid_Portal_Source()
        {

            var disambiguation = new List<DisambiguateFrom> { new DisambiguateFrom { Iswc = "T1" } };
            var workNumber = new WorkNumber { Number = "2" };
            var submission = new Submission();
            var lookUpManager = new Mock<ILookupManager>();
            var workManager = new Mock<IWorkManager>();

            submission.Model.WorkNumber = workNumber;
            submission.Model.PreferredIswc = "T203000239";
            submission.Model.Agency = "128";
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;
            submission.ExistingWork = new SubmissionModel() { DisambiguateFrom = disambiguation, Disambiguation = true };
            submission.RequestSource = RequestSource.PORTAL;

            lookUpManager.Setup(x => x.GetAgencyDisallowDisambiguationOverwrite("128")).ReturnsAsync("REST,CIS-NET,FILE");
            workManager.Setup(x => x.ExistsWithDisambiguation("T203000239", workNumber)).ReturnsAsync(true);

            var test = new Mock<IV_51>(GetMessagingManagerMock(ErrorCode._167), lookUpManager.Object, workManager.Object);
            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_51), test.Object.Identifier);
            lookUpManager.Verify(x => x.GetAgencyDisallowDisambiguationOverwrite(It.IsAny<string>()), Times.Once);
        }


        /// <summary>
        /// Check transaction passes if
        /// no existing disambiguation
        /// </summary>
        [Fact]
        public async void IV_51_Valid()
        {

            var disambiguation = new List<DisambiguateFrom> { new DisambiguateFrom { Iswc = "T1" } };
            var workNumber = new WorkNumber { Number = "2" };

            var submission = new Submission();
            var lookUpManager = new Mock<ILookupManager>();
            var workManager = new Mock<IWorkManager>();

            submission.ExistingWork = new SubmissionModel { Iswc = "" };

            submission.Model.WorkNumber = workNumber;
            submission.Model.PreferredIswc = "T203000239";
            submission.Model.Agency = "128";
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;
            submission.ExistingWork = new SubmissionModel() { };
            submission.RequestSource = RequestSource.FILE;

            lookUpManager.Setup(x => x.GetAgencyDisallowDisambiguationOverwrite("128")).ReturnsAsync("REST,CIS-NET,FILE");
            workManager.Setup(x => x.ExistsWithDisambiguation("31242142", workNumber)).ReturnsAsync(false);

            var test = new Mock<IV_51>(GetMessagingManagerMock(ErrorCode._167), lookUpManager.Object, workManager.Object);
            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_51), test.Object.Identifier);
            lookUpManager.Verify(x => x.GetAgencyDisallowDisambiguationOverwrite(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Check transaction fails if
        /// existing disambiguation on ISWC
        /// submitted by another eligible agency
        /// </summary>
        [Fact]
        public async void IV_51_InValid()
        {
            var submission = new Submission();
            var workNumber = new WorkNumber { Number = "2" };
            var lookUpManager = new Mock<ILookupManager>();
            var workManager = new Mock<IWorkManager>();

            submission.ExistingWork = new SubmissionModel { Iswc = "T203000239" };

            submission.Model.WorkNumber = workNumber;
            submission.Model.PreferredIswc = "";
            submission.Model.Agency = "128";
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;
            submission.RequestSource = RequestSource.FILE;

            lookUpManager.Setup(x => x.GetAgencyDisallowDisambiguationOverwrite("128")).ReturnsAsync("REST,CIS-NET,FILE");
            workManager.Setup(x => x.ExistsWithDisambiguation("T203000239", workNumber)).ReturnsAsync(true);

            var test = new Mock<IV_51>(GetMessagingManagerMock(ErrorCode._167), lookUpManager.Object, workManager.Object);
            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_51), test.Object.Identifier);
            Assert.Equal(ErrorCode._167, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if
        /// existing disambiguation on work being updated
        /// </summary>
        [Fact]
        public async void IV_51_InValid_Disambiguation_Already_Exists()
        {

            var disambiguation = new List<DisambiguateFrom> { new DisambiguateFrom { Iswc = "T1" } };
            var workNumber = new WorkNumber { Number = "2" };
            var submission = new Submission();
            var lookUpManager = new Mock<ILookupManager>();
            var workManager = new Mock<IWorkManager>();

            submission.Model.WorkNumber = workNumber;
            submission.Model.PreferredIswc = "T203000239";
            submission.Model.Agency = "128";
            submission.Model.Disambiguation = false;
            submission.TransactionType = Bdo.Edi.TransactionType.CUR;
            submission.RequestSource = RequestSource.FILE;
            submission.ExistingWork = new SubmissionModel { Disambiguation = true };


            lookUpManager.Setup(x => x.GetAgencyDisallowDisambiguationOverwrite("128")).ReturnsAsync("REST,CIS-NET,FILE");
            workManager.Setup(x => x.ExistsWithDisambiguation("T203000239", workNumber)).ReturnsAsync(true);
            var test = new Mock<IV_51>(GetMessagingManagerMock(ErrorCode._167), lookUpManager.Object, workManager.Object);

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_51), test.Object.Identifier);
            Assert.Equal(ErrorCode._167, response.Submission.Rejection.Code);
        }
    }
}
