using System.Collections.Generic;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Business.Tests.Models
{
    /// <summary>
    /// Tests Submission Model MD5 Method
    /// </summary>
    public class SubmissionModelTests
    {
        readonly Submission submission = new Submission
        {
            Model = new SubmissionModel
            {
                Titles = new List<Title>()
                {
                    new Title
                    {
                        Name = "TEST TITLE 1",
                        Type = TitleType.OT
                    },
                    new Title
                    {
                        Name = "TEST TITLE 2 ",
                        Type = TitleType.AT
                    }
                },
                InterestedParties = new List<InterestedPartyModel>()
                {
                    new InterestedPartyModel
                    {
                        IPNameNumber = 279271434,
                        Type = InterestedPartyType.C
                    },
                    new InterestedPartyModel
                    {
                        IPNameNumber = 136730972,
                        Type = InterestedPartyType.E
                    }
                }
            }
        };

        /// <summary>
        /// Tests MD5 Method with base Model
        /// </summary>
        [Fact]
        public void Validate_Submission_Model_MD5()
        {
            Assert.Equal("038b57191d3fbe5ef9e2cc4bf212f2da", submission.Model.GetHashValue());
        }

        /// <summary>
        /// Tests MD5 Method with no IPs
        /// </summary>
        [Fact]
        public void Validate_Submission_Model_MD5_NoIPs()
        {
            submission.Model.InterestedParties.Clear();

            Assert.Equal("c93c0deeb18879438d7cdadce5780fe9", submission.Model.GetHashValue());
        }

        /// <summary>
        /// Tests MD5 Method with base Model and Disambiguation
        /// </summary>
        [Fact]
        public void Validate_Submission_Model_MD5_With_Disambiguation()
        {
            submission.Model.DisambiguateFrom = new List<DisambiguateFrom>()
            {
                new DisambiguateFrom
                {
                    Iswc = "DISAMBIGUATION TEST 1"
                },
                new DisambiguateFrom
                {
                    Iswc = "DISAMBIGUATION TEST 2"
                }
            };

            submission.Model.Disambiguation = true;
            submission.Model.DisambiguationReason = DisambiguationReason.DIT;
            submission.Model.BVLTR = BVLTR.B;
            submission.Model.Instrumentation = new Instrumentation[] { new Instrumentation("ABC") };
            submission.Model.Performers = new Performer[] { new Performer { FirstName = "asdf", LastName = "dfjsio" } };

            Assert.Equal("ce8f489dc99e8c1dbf84201f72b92d46", submission.Model.GetHashValue());
        }

        /// <summary>
        /// Tests MD5 Method with null values returns hash without throwing exception
        /// </summary>
        [Fact]
        public void Validate_Submission_Model_MD5_Without_Title()
        {
            var submission = new Submission
            {
                Model = new SubmissionModel { }
            };

            Assert.Equal("f8320b26d30ab433c5a54546d21f414c", submission.Model.GetHashValue());
        }
    }
}
