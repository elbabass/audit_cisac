﻿using Moq;
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
    /// Tests rule IV_36
    /// </summary>
    public class IV_36Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if DerivedFrom(Composite) ISWC pattern is invalid
        /// </summary>
        [Fact]
        public async void IV_36_Submission_IswcPattern_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("T203000239", false, false)).ReturnsAsync(default(SubmissionModel));
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._119)).Object;

            submission.Model.DerivedWorkType = DerivedWorkType.Composite;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T203000239", Title = "Test" } };

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid); 
            Assert.Equal(nameof(IV_36), test.Identifier);
            Assert.Equal(ErrorCode._119, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if DerivedFrom(Composite) ISWC does not exist in the db
        /// </summary>
        [Fact]
        public async void IV_36_Submission_IswcNoInDb_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("T2030009990", false, false)).ReturnsAsync(default(SubmissionModel));
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._119)).Object;

			submission.Model.DerivedWorkType = DerivedWorkType.Composite;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009990", Title = "Test" } };

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_36), test.Identifier);
            Assert.Equal(ErrorCode._119, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if no DerivedFrom(Composite) ISWC or title is provided. 
        /// </summary>
        [Fact]
        public async void IV_36_Submission_NoTitleOrIswc_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("", false, false)).ReturnsAsync(default(SubmissionModel));
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._120)).Object; ;

			submission.Model.DerivedWorkType = DerivedWorkType.Composite;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "", Title = "" } };

			var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_36), test.Identifier);
            Assert.Equal(ErrorCode._120, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if DerivedWorkType is set to Composite and no DerivedFrom information is provided. "full" param
        /// </summary>
        [Fact]
        public async void IV_36_Submission_NoDF_InValid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("", false, false)).ReturnsAsync(default(SubmissionModel));
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._121)).Object;

			submission.Model.DerivedWorkType = DerivedWorkType.Composite;

            var response = await test.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_36), test.Identifier);
            Assert.Equal(ErrorCode._121, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if DerivedFrom(Composite) Iswc pattern is valid and it exists in the db. "full" param
        /// </summary>
        [Fact]
        public async void IV_36_Submission_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() {Agency = null, Iswc = "iswc1" });
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._119)).Object;

			submission.Model.DerivedWorkType = DerivedWorkType.Composite;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009903", Title = "Test" } };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_36), test.Identifier);
        }

        /// <summary>
        /// Check transaction passes if only DerivedFrom(Composite) title is provided. "full" param
        /// </summary>
        [Fact]
        public async void IV_36_Submission_HasTitleOnly_Valid()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() {Agency = null, Iswc = "iswc1" });
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._119)).Object;

			submission.Model.DerivedWorkType = DerivedWorkType.Composite;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "", Title = "Test" } };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_36), test.Identifier);
        }

		/// <summary>
		/// Check transaction passes if only DerivedFrom(Composite) ISWC is provided. 
		/// </summary>
		[Fact]
		public async void IV_36_Submission_HasIswcOnly_Valid()
		{
			var submission = new Submission() { Model = new SubmissionModel() };
			var workManager = new Mock<IWorkManager>();
			var rulesManagerMock = new Mock<IRulesManager>();

			workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() { Agency = null, Iswc = "iswc1" });
			rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(true);

			var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._119)).Object;

			submission.Model.DerivedWorkType = DerivedWorkType.Composite;
			submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "T2030009903", Title = "" } };

			var response = await test.IsValid(submission);

			Assert.True(response.IsValid);
		}

        /// <summary>
        /// Check transaction passes if rule is false
        /// </summary>
        [Fact]
        public async void IV_36_Submission_HasTitleOnly_Valid_Rule()
        {
            var submission = new Submission() { Model = new SubmissionModel() };
            var workManager = new Mock<IWorkManager>();
            var rulesManagerMock = new Mock<IRulesManager>();

            workManager.Setup(v => v.FindAsync("T2030009903", false, false)).ReturnsAsync(new SubmissionModel() { Agency = null, Iswc = "iswc1" });
            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateComposite")).ReturnsAsync(false);

            var test = new Mock<IV_36>(workManager.Object, rulesManagerMock.Object, GetMessagingManagerMock(ErrorCode._119)).Object;

            submission.Model.DerivedWorkType = DerivedWorkType.Composite;
            submission.Model.DerivedFrom = new List<DerivedFrom>() { new DerivedFrom() { Iswc = "", Title = "Test" } };

            var response = await test.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_36), test.Identifier);
        }
    }
}
