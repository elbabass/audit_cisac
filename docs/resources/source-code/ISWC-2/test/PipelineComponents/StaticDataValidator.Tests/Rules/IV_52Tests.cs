using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Tests;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Tests.Rules
{
    /// <summary>
    /// Tests rule IV_52
    /// </summary>
    public class IV_52Tests : TestBase
    {
        /// <summary>
        /// Check if duplicate submissions in batch and update the duplicate/s with rejection
        /// </summary>
        [Fact]
        public async Task IV_52_Check_If_Duplicate_Submissions()
        {
            var submissions = new List<Submission>{
                new Submission() { SubmissionId = 1, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_01", Type = "001" } } },
                new Submission() { SubmissionId = 2, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_01", Type = "001" } } },
                new Submission() { SubmissionId = 3, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_02", Type = "001" } } },
                new Submission() { SubmissionId = 2, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_01", Type = "001" } } }
                };

            var inputSubmissionsCount = submissions.Count();
            var test = new Mock<IV_52>(GetMessagingManagerMock(ErrorCode._170));

            var response = await test.Object.IsValid(submissions);

            Assert.Equal(nameof(IV_52), test.Object.Identifier);
            Assert.Collection(response,
               elem1 =>
               {
                   Assert.Null(elem1.Rejection);
               },
               elem2 =>
                {
                    Assert.NotNull(elem2.Rejection);
                    Assert.Equal(ErrorCode._170, elem2.Rejection.Code);
                },
               elem3 =>
                {
                    Assert.Null(elem3.Rejection);
                },
                elem4 =>
                {
                    Assert.NotNull(elem4.Rejection);
                    Assert.Equal(ErrorCode._170, elem4.Rejection.Code);
                });
            Assert.Equal(submissions.Count(), response.Count());
        }

        /// <summary>
        /// Check if no duplicate submissions in batch and submssion is not affected by rule
        /// </summary>
        [Fact]
        public async Task IV_52_Check_If_No_Duplicate_Submissions()
        {
            var submissions = new List<Submission>{
                new Submission() { SubmissionId = 1, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_01", Type = "001" } } },
                new Submission() { SubmissionId = 2, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_02", Type = "001" } } },
                new Submission() { SubmissionId = 3, Model = new SubmissionModel() { Agency = "001", WorkNumber = new Bdo.Work.WorkNumber { Number = "TEST_03", Type = "001" } } }
                };

            var test = new Mock<IV_52>(GetMessagingManagerMock(ErrorCode._170));

            var response = await test.Object.IsValid(submissions);

            Assert.Equal(nameof(IV_52), test.Object.Identifier);
            Assert.Collection(submissions,
               elem1 =>
               {
                   Assert.Null(elem1.Rejection);
               },
               elem2 =>
               {
                   Assert.Null(elem2.Rejection);
               },
               elem3 =>
               {
                   Assert.Null(elem3.Rejection);
               });
            Assert.Equal(3, submissions.Count());
        }
    }
}
