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
    /// Tests rule IV_38
    /// </summary>
    public class IV_40Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if no DisambiguateFrom ISWCs are provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_NoDisambiguateFromIswcsProvided_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._124), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_40), test.Object.Identifier);
            Assert.Equal(ErrorCode._124, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if the DisambiguateFrom ISWC(s) provided are not valid
        /// </summary>
        [Fact]
        public async void IV_40_Submission_InValidIswcProvided_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._122), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T003243423" } }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_40), test.Object.Identifier);
            Assert.Equal(ErrorCode._122, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if Performer is missing LastName
        /// </summary>
        [Fact]
        public async void IV_40_Submission_NoPerformerLastname_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._126), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } },
                Performers = new List<Performer>()
                    {
                        new Performer(){FirstName=string.Empty, LastName="LastName"},
                        new Performer(){FirstName="FirstName", LastName=string.Empty}
                    }
            };

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_40), test.Object.Identifier);
            Assert.Equal(ErrorCode._126, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if Performer has LastName
        /// </summary>
        [Fact]
        public async void IV_40_Submission_PerformersWithLastname_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { Model = new SubmissionModel() };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._126), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } },
                Performers = new List<Performer>()
                    {
                        new Performer(){FirstName=string.Empty, LastName="LastName"},
                        new Performer(){FirstName="FirstName", LastName="LastName"}
                    }
            };

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_40), test.Object.Identifier);
        }

        /// <summary>
        /// Check transaction passes if valid DisambiguateFrom ISWCs are provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_DisambiguateFromIswcsProvided_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._126), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if no DisambiguationReasonCode is provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_NoDisambiguationReasonCodeProvided_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._123), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._123, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if invalid DisambiguationReasonCode is provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_DisambiguationReasonCodeProvided_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._123), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                Disambiguation = true,
                DisambiguationReason = (DisambiguationReason)25,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._123, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if invalid BLTVR value is provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_BLTVR_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._125), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                BVLTR = (BVLTR)25,
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._125, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if valid BLTVR value is provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_BLTVR_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>().Object;
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._125), rulesManagerMock.Object, instrumentationManagerMock);

            submission.Model = new SubmissionModel()
            {
                BVLTR = BVLTR.B,
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.True(response.IsValid);
        }

        /// <summary>
        /// Check transaction fails if invalid instrumentation code is provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_InstrumentationCode_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>();
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            instrumentationManagerMock.Setup(i => i.GetInstrumentationByCodeAsync("ZZZ")).ReturnsAsync(default(Instrumentation));

            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._146), rulesManagerMock.Object, instrumentationManagerMock.Object);

            submission.Model = new SubmissionModel()
            {
                Instrumentation = new List<Instrumentation>() { new Instrumentation("ZZZ") },
                BVLTR = BVLTR.B,
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.False(response.IsValid);
            Assert.Equal(ErrorCode._146, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if valid instrumentation code is provided
        /// </summary>
        [Fact]
        public async void IV_40_Submission_InstrumentationCode_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var instrumentationManagerMock = new Mock<ILookupManager>();
            var submission = new Submission() { };

            rulesManagerMock.Setup(r => r.GetParameterValue<bool>("ValidateDisambiguationInfo")).ReturnsAsync(true);
            instrumentationManagerMock.Setup(i => i.GetInstrumentationByCodeAsync("ALP")).ReturnsAsync(new Instrumentation("ALP"));
            instrumentationManagerMock.Setup(i => i.GetInstrumentationByCodeAsync("BOD")).ReturnsAsync(new Instrumentation("BOD"));

            var test = new Mock<IV_40>(GetMessagingManagerMock(ErrorCode._146), rulesManagerMock.Object, instrumentationManagerMock.Object);

            submission.Model = new SubmissionModel()
            {
                Instrumentation = new List<Instrumentation>() { new Instrumentation("ALP"), new Instrumentation("BOD") },
                BVLTR = BVLTR.B,
                Disambiguation = true,
                DisambiguationReason = DisambiguationReason.DIC,
                DisambiguateFrom = new List<DisambiguateFrom>() { new DisambiguateFrom() { Iswc = "T2030009903" } }
            };

            var response = await test.Object.IsValid(submission);
            Assert.True(response.IsValid);
        }
    }
}
