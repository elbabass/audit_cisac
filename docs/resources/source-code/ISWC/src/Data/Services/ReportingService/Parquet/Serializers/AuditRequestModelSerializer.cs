using Parquet.Data;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Data.Services.ReportingService.Parquet.Serializers
{
    internal class AuditRequestModelSerializer
    {
        public AuditRequestModelSerializer(IEnumerable<AuditRequestModel> records)
        {
            Columns = new List<DataColumn>
            {
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.AuditRequestId), typeof(string)),
                    records.Select(x => x.AuditRequestId.ToString()).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.AuditId), typeof(string)),
                    records.Select(x => x.AuditId.ToString()).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.RecordId), typeof(int)),
                    records.Select(x => x.RecordId).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.AgencyCode), typeof(string)),
                    records.Select(x => x.AgencyCode).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.CreatedDate), typeof(DateTimeOffset?)),
                    records.Select(x => (DateTimeOffset?)x.CreatedDate).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.IsProcessingError), typeof(bool)),
                    records.Select(x => x.IsProcessingError).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.IsProcessingFinished), typeof(bool)),
                    records.Select(x => x.IsProcessingFinished).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.TransactionError.Code), typeof(string)),
                    records.Select(x => x.TransactionError?.Code.ToFriendlyString()).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.TransactionError.Message), typeof(string)),
                    records.Select(x => x.TransactionError?.Message).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.TransactionType), typeof(string)),
                    records.Select(x => x.TransactionType.ToFriendlyString()).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.Work.PreferredIswc), typeof(string)),
                    records.Select(x => x.Work.PreferredIswc).ToArray()),
                new DataColumn(
                    new DataField("AgencyWorkCode", typeof(string)),
                    records.Select(x=> x.Work.WorkNumber.Number).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.Work.SourceDb), typeof(int)),
                    records.Select(x => x.Work.SourceDb).ToArray()),
                new DataColumn(
                    new DataField("OriginalTitle", typeof(string)),
                    records.Select(x => x.Work.Titles.FirstOrDefault(x=>x.Type == TitleType.OT)?.Name).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.TransactionSource), typeof(string)),
                    records.Select(x => x.Work.AdditionalIdentifiers != null && x.Work.AdditionalIdentifiers.Any() ? TransactionSource.Publisher.ToFriendlyString()
                    : TransactionSource.Agency.ToFriendlyString()).ToArray()),
                new DataColumn(
                    new DataField("CreatorNames", typeof(string)),
                    records.Select(x => x.Work.InterestedParties.Any() ? SerializeCreatorNames(x.Work.InterestedParties) : string.Empty).ToArray()),
                new DataColumn(
                    new DataField("CreatorNameNumbers", typeof(string)),
                    records.Select(x => x.Work.InterestedParties.Any() ? SerializeCreatorNameNumbers(x.Work.InterestedParties) : string.Empty).ToArray()),
                new DataColumn(
                    new DataField("PublisherNameNumber", typeof(long?)),
                    records.Select(x =>  x.Work.AdditionalIdentifiers != null && x.Work.AdditionalIdentifiers.Any() ? x.Work.AdditionalIdentifiers.FirstOrDefault(x => x?.NameNumber != null)?.NameNumber : 0).ToArray()),
                new DataColumn(
                    new DataField("PublisherWorkNumber", typeof(string)),
                    records.Select(x => x.Work.AdditionalIdentifiers != null && x.Work.AdditionalIdentifiers.Any() ? x.Work.AdditionalIdentifiers.FirstOrDefault(x => x?.NameNumber != null)?.WorkCode  : string.Empty).ToArray()),
                new DataColumn(
                    new DataField(nameof(AuditRequestModel.RelatedSubmissionIncludedIswc), typeof(bool)),
                    records.Select(x => x.RelatedSubmissionIncludedIswc).ToArray()),
                 new DataColumn(
                    new DataField("AgentVersion", typeof(string)),
                    records.Select(x => string.IsNullOrWhiteSpace(x.AgentVersion) ? x.AgentVersion : string.Empty).ToArray())


            };
        }

        public Schema Schema { get { return new Schema(Fields); } }
        public IReadOnlyCollection<DataField> Fields { get { return Columns.Select(x => x.Field).ToList().AsReadOnly(); } }
        public ICollection<DataColumn> Columns { get; }

        private string SerializeCreatorNames(IEnumerable<InterestedPartyModel> ips)
            => StringExtensions.CreateSemiColonSeperatedString(ips.Any(x => creatorTypes.Contains(x.CisacType)) ?
                ips.Where(c => creatorTypes.Contains(c.CisacType))?.Select(x => $"{x?.Name} {x?.LastName}") : default);

        private string SerializeCreatorNameNumbers(IEnumerable<InterestedPartyModel> ips)
            => StringExtensions.CreateSemiColonSeperatedString(ips.Any(x => creatorTypes.Contains(x.CisacType))
                ? ips.Where(c => creatorTypes.Contains(c.CisacType))?.Select(x => x?.IPNameNumber.ToString()) : default);

        private readonly List<CisacInterestedPartyType?> creatorTypes = new List<CisacInterestedPartyType?> { CisacInterestedPartyType.C, CisacInterestedPartyType.MA, CisacInterestedPartyType.TA };
    }
}
