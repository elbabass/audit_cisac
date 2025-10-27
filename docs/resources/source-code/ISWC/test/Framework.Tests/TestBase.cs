using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Framework.Tests
{
    /// <summary>
    /// Test base class
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// Returns a mock IMessagingManager configured to return a Rejection for the given error code.
        /// </summary>
        /// <param name="errorCode">The error code to return in the Rejection</param>
        /// <returns>Mocked IMessagingManager</returns>
        public IMessagingManager GetMessagingManagerMock(ErrorCode errorCode)
        {
            var messagingManagerMock = new Mock<IMessagingManager>();
            messagingManagerMock.Setup(x => x.GetRejectionMessage(errorCode))
                                .ReturnsAsync(new Rejection(errorCode, string.Empty));
            return messagingManagerMock.Object;
        }

        /// <summary>
        /// Returns a mock IWorkManager configured to return the provided list of ISWC models
        /// when FindManyIswcModelsAsync is called.
        /// </summary>
        /// <param name="iswcModels">List of ISWC models to return (optional)</param>
        /// <returns>Mocked IWorkManager</returns>
        public Mock<IWorkManager> GetWorkManagerMock(List<IswcModel> iswcModels = null)
        {
            var mock = new Mock<IWorkManager>();
            mock.Setup(x => x.FindManyIswcModelsAsync(
                        It.IsAny<IEnumerable<string>>(),
                        It.IsAny<bool>(),
                        It.IsAny<DetailLevel>()
                    ))
                .ReturnsAsync(iswcModels ?? new List<IswcModel>());
            return mock;
        }

        /// <summary>
        /// Returns a mock IInterestedPartyManager configured to return the provided list
        /// of InterestedPartyModel when FindManyByBaseNumber is called.
        /// </summary>
        /// <param name="ips">List of InterestedPartyModel to return (optional)</param>
        /// <returns>Mocked IInterestedPartyManager</returns>
        public Mock<IInterestedPartyManager> GetInterestedPartyManagerMock(List<InterestedPartyModel> ips = null)
        {
            var mock = new Mock<IInterestedPartyManager>();
            mock.Setup(x => x.FindManyByBaseNumber(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(ips ?? new List<InterestedPartyModel>());
            return mock;
        }

        /// <summary>
        /// Returns a mock IAgreementManager configured to return the provided list of
        /// agreements when FindManyAsync is called.
        /// </summary>
        /// <param name="agreements">List of agreements to return (optional)</param>
        /// <returns>Mocked IAgreementManager</returns>
        public Mock<IAgreementManager> GetAgreementManagerMock(List<Agreement> agreements = null)
        {
            var mock = new Mock<IAgreementManager>();
            mock.Setup(x => x.FindManyAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(agreements ?? new List<Agreement>());
            return mock;
        }
    }
}
