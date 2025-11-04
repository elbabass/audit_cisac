using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models
{
    public class CacheIswcMetadata
    {
        [JsonProperty("Iswc", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Iswc { get; set; }

        [JsonProperty("IswcStatus", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string IswcStatus { get; set; }

        [JsonProperty("Agency", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Agency { get; set; }

        [JsonProperty("OriginalTitle", Required = Required.Always)]
        public string OriginalTitle { get; set; }

        [JsonProperty("OtherTitles", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Title> OtherTitles { get; set; }

        [JsonProperty("InterestedParties", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
        public List<InterestedParty> InterestedParties { get; set; } = new List<InterestedParty>();

        [JsonProperty(PropertyName = "Recordings")]
        public List<Recording> Recordings { get; set; }

        [JsonProperty("ParentISWC", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ParentISWC { get; set; }

        [JsonProperty("OverallParentISWC", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string OverallParentISWC { get; set; }

        [JsonProperty("LinkedISWC", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> LinkedISWC { get; set; }

        [JsonProperty("CreatedDate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? CreatedDate { get; set; }

        [JsonProperty("LastModifiedDate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? LastModifiedDate { get; set; }

        [JsonProperty("Works", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Work> Works { get; set; }
    }

    public class InterestedParty
    {
        [JsonProperty(PropertyName = "Role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "NameNumber")]
        public long? IPNameNumber { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }

        [JsonProperty("Affiliation", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Affiliation { get; set; }

        [JsonProperty(PropertyName = "LegalEntityType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LegalEntityType LegalEntityType { get; set; }
    }

    public class Work
    {
        public string Agency { get; set; }
        public string WorkCode { get; set; }
    }

    public class Title
    {
        [JsonProperty("Title", Required = Required.Always)]
        public string Title1 { get; set; }

        [JsonProperty("Type", Required = Required.Always)]
        public string Type { get; set; }

        private IDictionary<string, object> _additionalProperties = new Dictionary<string, object>();

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    }

    public class Recording
    {
        [JsonProperty(PropertyName = "ISRC")]
        public string Isrc { get; set; }

        [JsonProperty(PropertyName = "Recording Title")]
        public string RecordingTitle { get; set; }

        [JsonProperty(PropertyName = "SubTitle")]
        public string SubTitle { get; set; }

        [JsonProperty(PropertyName = "LabelName")]
        public string LabelName { get; set; }

        [JsonProperty(PropertyName = "ReleaseEmbargoDate")]
        public DateTimeOffset? ReleaseEmbargoDate { get; set; }

        [JsonProperty(PropertyName = "Performers")]
        public List<Performer> Performers { get; set; }
    }

    public class Performer
    {
        [JsonProperty(PropertyName = "Isni")]
        public string Isni { get; set; }

        [JsonProperty(PropertyName = "Ipn")]
        public long? Ipn { get; set; }

        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "Designation")]
        public string Designation { get; set; }
    }
}