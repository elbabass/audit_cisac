using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_34
    /// </summary>
    public class IV_34Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if DerivedFrom(ModifiedVersion) ISWC pattern is invalid. "basic" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Basic_IswcPattern_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("basic");
            workManager.Setup(v => v.FindAsync("T203000240", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
			submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T203000240", Title="Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
            Assert.Equal(ErrorCode._119, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if DerivedFrom(ModifiedVersion) ISWC does not exist in the db. "basic" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Basic_IswcNoInDb_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("basic");
            workManager.Setup(v => v.FindAsync("T2030009999", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009999", Title = "Test" } };

			var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
            Assert.Equal(ErrorCode._119, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if DerivedFrom(ModifiedVersion) ISWC pattern is valid and it exists in the database. "basic" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Basic_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("basic");
            workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() { Agency = null, Iswc = "iswc1" });

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009903", Title = "Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
        }


        /// <summary>
        /// Check transaction passes if DerivedFrom(ModifiedVersion) ISWC empty and Title populated. "basic" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Basic_Empty_ISWC_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("basic");
            workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() { Agency = null, Iswc = "iswc1" });

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() {  Title = "Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction fails if DerivedFrom(ModifiedVersion) ISWC pattern is invalid. "full" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Full_IswcPattern_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
            workManager.Setup(v => v.FindAsync("T203000239", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T203000239", Title = "Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
            Assert.Equal(ErrorCode._119, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if DerivedFrom(ModifiedVersion) ISWC does not exist in the db. "full" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Full_IswcNoInDb_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
            workManager.Setup(v => v.FindAsync("T2030009999", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009999", Title = "Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
            Assert.Equal(ErrorCode._119, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if DerivedFrom(ModifiedVersion) ISWC pattern is valid and it exists in the database. "full" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Full_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
            workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() { Agency = null, Iswc = "iswc1" });

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009903", Title = "Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if only DerivedFrom(ModifiedVersion) title is provided. "full" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Full_HasTitleOnly_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
            workManager.Setup(v => v.FindAsync("", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._118));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "", Title = "Test" } };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
        }

		/// <summary>
		/// Check transaction passes if only DerivedFrom(ModifiedVersion) ISWC is provided. "full" parameter
		/// </summary>
		[Fact]
		public async void IV_34_Submission_Full_HasIswcOnly_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel() };
			var rulesManagerMock = new Mock<IRulesManager>();
			var workManager = new Mock<IWorkManager>();

			rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
			workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() { Agency = null, Iswc = "iswc1" });

			var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._118));

			submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
			submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009903", Title = "" } };

			var response = await test.Object.IsValid(submission);

			Assert.True(response.IsValid);
			Assert.Equal(nameof(IV_34), test.Object.Identifier);
		}

        /// <summary>
        /// Check transaction fails if DerivedWorkType is set to ModifiedVersion and no DerivedFrom information is provided. "full" param
        /// </summary>
        [Fact]
        public async void IV_34_Submission_NoDF_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
            workManager.Setup(v => v.FindAsync("", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._118));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
            Assert.Equal(ErrorCode._118, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if no DerivedFrom(ModifiedVersion) ISWC or title is provided. "full" parameter
        /// </summary>
        [Fact]
        public async void IV_34_Submission_Full_NoTitleOrIswc_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("full");
            workManager.Setup(v => v.FindAsync("T2030009999", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._120));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "", Title = "" } };

			var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
            Assert.Equal(ErrorCode._120, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes when parameter is set to "none"
        /// </summary>
        [Fact]
        public async void IV_34_Submission_None_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var rulesManagerMock = new Mock<IRulesManager>();
            var workManager = new Mock<IWorkManager>();

            rulesManagerMock.Setup(p => p.GetParameterValue<string>("ValidateModifiedVersions")).ReturnsAsync("none");
            workManager.Setup(v => v.FindAsync("", false, false)).ReturnsAsync(default(SubmissionModel));

            var test = new Mock<IV_34>(rulesManagerMock.Object, workManager.Object, GetMessagingManagerMock(ErrorCode._119));

            submission.Model.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "", Title = "Test" } };

			var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_34), test.Object.Identifier);
        }
    }
}
