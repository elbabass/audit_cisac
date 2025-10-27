using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Business.Validators;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Business.Tests.Validators
{
    /// <summary>
    /// Tests BaseValidator
    /// </summary>
    public class BaseValidatorTests
    {
        readonly Mock<IRulesManager> rulesManager = new Mock<IRulesManager>();
        
        /// <summary>
        /// Check ValidateBatch returns all Submissions
        /// </summary>
        [Fact]
        public async void ValidateBatch_ReturnsAllSubmissions()
        {
            var batch = new List<Submission>() { new Submission() };
            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, It.IsAny<ValidatorType>()).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            Assert.Equal(batch.Count(), matchedBatch.Count());
        }

        /// <summary>
        /// Check ValidateBatch returns all valid Submissions
        /// </summary>
        [Fact]
        public async void ValidateBatch_Returns_ValidSubmissions()
        {
            var submission = new Submission() { TransactionType = TransactionType.CAR };
            var rule = new Mock<IRule>(); var batch = new List<Submission>() { submission };

            rule.Setup(x => x.IsValid(submission)).ReturnsAsync((IsValid: true, Submission: submission));
            rule.Setup(x => x.ValidTransactionTypes).Returns(new TransactionType[] { TransactionType.CAR });
            rulesManager.Setup(x => x.GetEnabledRules<IRule>()).ReturnsAsync(new IRule[] { rule.Object });

            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, It.IsAny<ValidatorType>()).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            Assert.Equal(batch.Count(), matchedBatch.Count());
            Assert.Equal(submission, matchedBatch.First());
        }

        /// <summary>
        /// Check ValidateBatch returns all invalid Submissions
        /// </summary>
        [Fact]
        public async void ValidateBatch_Returns_InvalidSubmissions()
        {
            var submission = new Submission() { TransactionType = TransactionType.CAR };
            var rule = new Mock<IRule>(); var batch = new List<Submission>() { submission };

            rule.Setup(x => x.IsValid(submission)).ReturnsAsync((IsValid: false, Submission: null));
            rule.Setup(x => x.ValidTransactionTypes).Returns(new TransactionType[] { TransactionType.CAR });
            rulesManager.Setup(x => x.GetEnabledRules<IRule>()).ReturnsAsync(new IRule[] { rule.Object });

            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, It.IsAny<ValidatorType>()).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            Assert.Equal(batch.Count(), matchedBatch.Count());
            Assert.Null(matchedBatch.First());
        }

        /// <summary>
        /// Check ValidateBatch returns Submission with different TransactionType
        /// </summary>
        [Fact]
        public async void ValidateBatch_SubmissionWithDifferentTransactionType()
        {
            var submission = new Submission() { TransactionType = TransactionType.CAR };
            var rule = new Mock<IRule>(); var batch = new List<Submission>() { submission };

            rule.Setup(x => x.ValidTransactionTypes).Returns(new TransactionType[] { TransactionType.CUR });
            rulesManager.Setup(x => x.GetEnabledRules<IRule>()).ReturnsAsync(new IRule[] { rule.Object });

            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, It.IsAny<ValidatorType>()).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            Assert.Equal(batch.Count(), matchedBatch.Count());
            rule.Verify(x => x.IsValid(submission), Times.Never);
        }


        /// <summary>
        /// Check ValidateBatch returns Submission with same ValidatorType
        /// </summary>
        [Fact]
        public async void ValidateBatch_SubmissionWithSameValidatorType()
        {
            var submission = new Submission() { TransactionType = TransactionType.CUR };
            var rule = new Mock<IRule>(); var batch = new List<Submission>() { submission };

            rule.Setup(x => x.ValidTransactionTypes).Returns(new TransactionType[] { TransactionType.CUR });
            rule.Setup(x => x.ValidatorType).Returns(ValidatorType.IswcEligibiltyValidator);
            rulesManager.Setup(x => x.GetEnabledRules<IRule>()).ReturnsAsync(new IRule[] { rule.Object });

            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, ValidatorType.IswcEligibiltyValidator).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            Assert.Equal(batch.Count(), matchedBatch.Count());
            rule.Verify(x => x.IsValid(submission), Times.Once);
        }

        /// <summary>
        /// Check ValidateBatch returns Submission with different ValidatorType
        /// </summary>
        [Fact]
        public async void ValidateBatch_SubmissionWithDifferentValidatorType()
        {
            var submission = new Submission() { TransactionType = TransactionType.CUR };
            var rule = new Mock<IRule>(); var batch = new List<Submission>() { submission };

            rule.Setup(x => x.ValidTransactionTypes).Returns(new TransactionType[] { TransactionType.CUR });
            rule.Setup(x => x.ValidatorType).Returns(ValidatorType.LookupDataValidator);
            rulesManager.Setup(x => x.GetEnabledRules<IRule>()).ReturnsAsync(new IRule[] { rule.Object });

            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, ValidatorType.IswcEligibiltyValidator).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            Assert.Equal(batch.Count(), matchedBatch.Count());
            rule.Verify(x => x.IsValid(submission), Times.Never);
        }

        /// <summary>
        /// Check ValidateBatch doesn't validate if there is a rejecion
        /// </summary>
        [Fact]
        public async void ValidateBatch_IfRejectionIsPresentDontValidate()
        {
            var submission = new Submission() { Rejection = new Rejection(ErrorCode._100, string.Empty) };
            var rule = new Mock<IRule>(); var batch = new List<Submission>() { submission };

            rulesManager.Setup(x => x.GetEnabledRules<IRule>()).ReturnsAsync(new IRule[] { rule.Object });

            var baseValidator = new Mock<BaseValidator>(rulesManager.Object, It.IsAny<ValidatorType>()).Object;

            var matchedBatch = await baseValidator.ValidateBatch(batch);

            rule.Verify(x => x.IsValid(submission), Times.Never);
            Assert.NotNull(matchedBatch.First().Rejection);
        }
    }
}
