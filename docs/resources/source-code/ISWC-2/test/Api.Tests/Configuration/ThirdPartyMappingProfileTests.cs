using AutoMapper;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Configuration;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Configuration
{
    /// <summary>
    /// Unit tests for SpanishPoint.Azure.Iswc.Api.Label.Configuration.ThirdPartyMappingProfile
    /// </summary>
    public class ThirdPartyMappingProfileTests
    {
        /// <summary>
        /// Tests that ThirdParty.V1.InterestedParty maps correctly to an InterestedPartyModel.
        /// </summary>
        [Fact]
        public void Map_V1_InterestedParty_to_InterestedPartyModel()
        {
            // Arrange
            var myProfile = new ThirdPartyMappingProfile();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetIndexParameters().Length == 0 && p.GetMethod.IsPublic;
                cfg.AddProfile(myProfile);
            });
            var mapper = new Mapper(configuration);

            var interestedParties = new List<ThirdParty.V1.InterestedParty>
            {
                new ThirdParty.V1.InterestedParty
                {
                    NameNumber = 123456781
                },
                new ThirdParty.V1.InterestedParty
                {
                    NameNumber = 123456782,
                    Role = ThirdParty.V1.InterestedPartyRole.E
                },
                new ThirdParty.V1.InterestedParty
                {
                    NameNumber = 123456783
                }
            };

            // Act
            var interestedPartyModels = mapper.Map<List<InterestedPartyModel>>(interestedParties);

            // Assert
            Assert.Equal(interestedParties.Count, interestedPartyModels.Count);
            Assert.Equal(interestedParties[0].NameNumber, interestedPartyModels[0].IPNameNumber);
            Assert.Equal(InterestedPartyType.C, interestedPartyModels[0].Type);
            Assert.Equal(interestedParties[1].NameNumber, interestedPartyModels[1].IPNameNumber);
            Assert.Equal(InterestedPartyType.E, interestedPartyModels[1].Type);
            Assert.Equal(interestedParties[2].NameNumber, interestedPartyModels[2].IPNameNumber);
            Assert.Equal(InterestedPartyType.C, interestedPartyModels[2].Type);
        }
    }
}
