using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SpanishPoint.Azure.Iswc.Data.Extensions;
using SpanishPoint.Azure.Iswc.Data.Tests.TestFactories;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Extensions
{
    /// <summary>
    /// Test DbContextExtensions extension methods.
    /// </summary>
    public class ExtensionTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<DbSet<DataModels.Iswc>> _mockIswc;
        private readonly Mock<DbSet<DataModels.IswclinkedTo>> _mockIswclinkedTo;
        private readonly DataModels.Iswc _baseLevelIswc = new DataModels.Iswc { IswcId = 6919022723, Iswc1 = "T9805234406" };

        /// <summary>
        /// Set up
        /// </summary>
        public ExtensionTests()
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);

            var iswclinkedToData = new List<DataModels.IswclinkedTo> {
                new DataModels.IswclinkedTo { LinkedToIswc = "T9805214066", IswcId = 6919020686, Iswc = new DataModels.Iswc { IswcId = 6919020686, Iswc1 = "T9805214077" } ,Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T9805214066", IswcId = 6919020687, Iswc = new DataModels.Iswc { IswcId = 6919020687, Iswc1 = "T9805214088" } ,Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T9805214066", IswcId = 6919020688, Iswc = new DataModels.Iswc { IswcId = 6919020688, Iswc1 = "T9805214099" } ,Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T9805214124", IswcId = 6919020685, Iswc = new DataModels.Iswc { IswcId = 6919020685, Iswc1 = "T9805214066" } ,Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T9805234406", IswcId = 6919020691, Iswc = new DataModels.Iswc { IswcId = 6919020691, Iswc1 = "T9805214124" } ,Status = true},
                
                // A self referencing IswclinkedTo.
                new DataModels.IswclinkedTo { LinkedToIswc = "T666-13", IswcId = 666, Iswc = new DataModels.Iswc { IswcId = 666, Iswc1 = "T666-13" }, Status = true }, 

                // Last IswclinkedTo of a chain is linked back to the first IswclinkedTo.
                new DataModels.IswclinkedTo { LinkedToIswc = "T1", IswcId = 2, Iswc = new DataModels.Iswc { IswcId = 2, Iswc1 = "T2" }, Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T2", IswcId = 3, Iswc = new DataModels.Iswc { IswcId = 3, Iswc1 = "T3" }, Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T3", IswcId = 4, Iswc = new DataModels.Iswc { IswcId = 4, Iswc1 = "T4" }, Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T4", IswcId = 5, Iswc = new DataModels.Iswc { IswcId = 5, Iswc1 = "T5" }, Status = true},
                new DataModels.IswclinkedTo { LinkedToIswc = "T5", IswcId = 1, Iswc = new DataModels.Iswc { IswcId = 1, Iswc1 = "T1" }, Status = true}
            }.AsQueryable();

            var iswcData = new List<DataModels.Iswc> {
                new DataModels.Iswc { IswcId = 6919020685, Iswc1 = "T9805214066" },
                new DataModels.Iswc { IswcId = 6919020686, Iswc1 = "T9805214077" },
                new DataModels.Iswc { IswcId = 6919020687, Iswc1 = "T9805214088" },
                new DataModels.Iswc { IswcId = 6919020688, Iswc1 = "T9805214099" },
                new DataModels.Iswc { IswcId = 6919020691, Iswc1 = "T9805214124" },
                _baseLevelIswc,
                new DataModels.Iswc { IswcId = 666, Iswc1 = "T666-13" }
            };

            for (int i = 1; i <= 5; i++)
            {
                iswcData.Add(new DataModels.Iswc { IswcId = i, Iswc1 = $"T{i}" });
            }

            _mockIswc = MockDbSetBuilder.MockDbSetFactory<DataModels.Iswc>(iswcData.AsQueryable());
            _mockIswclinkedTo = MockDbSetBuilder.MockDbSetFactory<DataModels.IswclinkedTo>(iswclinkedToData);
        }

        /*
         * The value of the overallParentISWC set by FindOverallParentIswc() should be the ISWC at the top of 
         * the chain of ISWCs rather than the ISWC of the immediate parent as with the parentISWC property.
         */

        /// <summary>
        /// Check when passed ISWC's at different layers the overall parent is returned.
        /// </summary>
        [Theory]
        [MemberData(nameof(IswcListToCheck))]
        public void FindOverallParentIswc_ShouldSetOverallParentIswcToBaseIswc(Bdo.Iswc.IswcModel iswcToCheck)
        {
            // Act
            iswcToCheck.OverallParentIswc = iswcToCheck.FindOverallParentIswc(_mockIswclinkedTo.Object, _mockIswc.Object, _mapper);

            // Assert
            Assert.NotNull(iswcToCheck);
            Assert.NotNull(iswcToCheck.OverallParentIswc);
            Assert.Equal(_baseLevelIswc.Iswc1, iswcToCheck.OverallParentIswc.Iswc);
        }

        /// <summary>
        /// Check when passed ISWC at top level FindOverallParentIswc() returns null when it is checked.
        /// </summary>
        [Fact]
        public void FindOverallParentIswc_ShouldReturnNull()
        {
            // Set up
            var iswcToCheck = new Bdo.Iswc.IswcModel { IswcId = 6919022723, Iswc = "T9805234406" };

            // Act
            iswcToCheck.OverallParentIswc = iswcToCheck.FindOverallParentIswc(_mockIswclinkedTo.Object, _mockIswc.Object, _mapper);

            // Assert
            Assert.NotNull(iswcToCheck);
            Assert.Null(iswcToCheck.OverallParentIswc);
        }

        /// <summary>
        /// FindOverallParentIswc() should be able to handle a self referencing IswclinkedTo without going into an infinite loop and crashing.
        /// </summary>
        [Theory]
        [MemberData(nameof(IswcListToCheckForSelfReferencing))]
        public void FindOverallParentIswc_IswclinkedToIsSelfReferencing_ThrowsInvalidOperationException(Bdo.Iswc.IswcModel iswcToCheck)
        {
            // Act & Assert
            Assert.Null(iswcToCheck.FindOverallParentIswc(_mockIswclinkedTo.Object, _mockIswc.Object, _mapper));
        }

        /// <summary>
        /// Handle null ISWC parameter.
        /// </summary>
        [Fact]
        public void FindOverallParentIswc_IswcIsNull_ThrowsArgumentNullException()
        {
            // Set up
            Bdo.Iswc.IswcModel iswcToCheck = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => iswcToCheck.FindOverallParentIswc(_mockIswclinkedTo.Object, _mockIswc.Object, _mapper));
        }

        /// <summary>
        /// Set up list of IWSC's to check for parent.
        /// </summary>
        public static IEnumerable<object[]> IswcListToCheck()
        {
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 6919020686, Iswc = "T9805214077" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 6919020687, Iswc = "T9805214088" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 6919020688, Iswc = "T9805214099" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 6919020685, Iswc = "T9805214066" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 6919020691, Iswc = "T9805214124" } };
        }

        /// <summary>
        /// Set up list of IWSC's to check for self referencing.
        /// </summary>
        public static IEnumerable<object[]> IswcListToCheckForSelfReferencing()
        {
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 666, Iswc = "T666-13" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 5, Iswc = "T5" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 4, Iswc = "T4" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 3, Iswc = "T3" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 2, Iswc = "T2" } };
            yield return new object[] { new Bdo.Iswc.IswcModel { IswcId = 1, Iswc = "T1" } };
        }
    }
}
