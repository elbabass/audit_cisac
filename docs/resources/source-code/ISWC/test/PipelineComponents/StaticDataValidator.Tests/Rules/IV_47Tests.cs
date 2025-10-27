using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_47
    /// </summary>
    public class IV_47Tests : TestBase
    {
        /// <summary>
        /// Check transaction fails if any Performer Last Name exceeds 50 characters
        /// </summary>
        [Fact]
        public async void IV_11_Submission_Performer_LastName_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Performers = new List<Performer>
                    {
                        new Performer { FirstName = "firstname", LastName = "lastname" },
                        new Performer { FirstName = "firstname2", LastName = "lastname2" },
                        new Performer { FirstName = "", LastName = "lastnamedfsfsdfsdfsdfsdfsdfsfdsfsfsfsfsdfssfdsfssf1" }
                    }
                }
            };

            var test = new Mock<IV_47>(GetMessagingManagerMock(ErrorCode._162));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_47), test.Object.Identifier);
            Assert.Equal(ErrorCode._162, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if any Performer First Name exceeds 50 characters
        /// </summary>
        [Fact]
        public async void IV_11_Submission_Performer_FirstName_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Performers = new List<Performer>
                    {
                        new Performer { FirstName = "firstname", LastName = "lastname" },
                        new Performer { FirstName = "firstname2", LastName = "lastname2" },
                        new Performer { FirstName = "firstnamedfsfsdfsdfsdfsdfsdfsfdsfsfsfsfsdfssfdsfssf1", LastName = "lastname3" }
                    }
                }
            };

            var test = new Mock<IV_47>(GetMessagingManagerMock(ErrorCode._162));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_47), test.Object.Identifier);
            Assert.Equal(ErrorCode._162, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction fails if any Performer does not have a Last Name
        /// </summary>
        [Fact]
        public async void IV_11_Submission_Performer_No_Lastname_InValid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Performers = new List<Performer>
                    {
                        new Performer { FirstName = "firstname", LastName = "" },
                        new Performer { FirstName = "firstname2", LastName = "lastname2" },
                        new Performer { FirstName = "firstname3", LastName = "lastname3" }
                    }
                }
            };

            var test = new Mock<IV_47>(GetMessagingManagerMock(ErrorCode._126));

            var response = await test.Object.IsValid(submission);

            Assert.False(response.IsValid);
            Assert.Equal(nameof(IV_47), test.Object.Identifier);
            Assert.Equal(ErrorCode._126, response.Submission.Rejection.Code);
        }

        /// <summary>
        /// Check transaction passes if all Performers First and Last Names do not exceed 50 characters
        /// </summary>
        [Fact]
        public async void IV_11_Submission_Valid()
        {
            var rulesManagerMock = new Mock<IRulesManager>();
            var submission = new Submission()
            {
                Model = new SubmissionModel()
                {
                    Performers = new List<Performer>
                    {
                        new Performer { FirstName = "firstname", LastName = "50characters-1234567890123456677756565656511111111" },
                        new Performer { FirstName = "50characters-1234567890123456677756565656511111111", LastName = "lastname2" }
                    }
                }
            };

            var test = new Mock<IV_47>(GetMessagingManagerMock(ErrorCode._162));

            var response = await test.Object.IsValid(submission);

            Assert.True(response.IsValid);
            Assert.Equal(nameof(IV_47), test.Object.Identifier);
        }
    }
}
