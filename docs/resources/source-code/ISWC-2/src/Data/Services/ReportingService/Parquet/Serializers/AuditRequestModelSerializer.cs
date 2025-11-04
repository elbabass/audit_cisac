using Parquet.Schema;
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
                    new DataField<string>(nameof(AuditRequestModel.AuditRequestId)),
                    records.Select(x => x.AuditRequestId.ToString()).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.AuditId)),
                    records.Select(x => x.AuditId.ToString()).ToArray()),
                new DataColumn(
                    new DataField<int>(nameof(AuditRequestModel.RecordId)),
                    records.Select(x => x.RecordId).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.AgencyCode)),
                    records.Select(x => x.AgencyCode).ToArray()),
                new DataColumn(
                    new DataField<DateTimeOffset?>(nameof(AuditRequestModel.CreatedDate)),
                    records.Select(x => (DateTimeOffset?)x.CreatedDate).ToArray()),
                new DataColumn(
                    new DataField<bool>(nameof(AuditRequestModel.IsProcessingError)),
                    records.Select(x => x.IsProcessingError).ToArray()),
                new DataColumn(
                    new DataField<bool>(nameof(AuditRequestModel.IsProcessingFinished)),
                    records.Select(x => x.IsProcessingFinished).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.TransactionError.Code)),
                    records.Select(x => x.TransactionError?.Code.ToFriendlyString()).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.TransactionError.Message)),
                    records.Select(x => x.TransactionError?.Message).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.TransactionType)),
                    records.Select(x => x.TransactionType.ToFriendlyString()).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.Work.PreferredIswc)),
                    records.Select(x => x.Work.PreferredIswc).ToArray()),
                new DataColumn(
                    new DataField<string>("AgencyWorkCode"),
                    records.Select(x=> x.Work.WorkNumber.Number).ToArray()),
                new DataColumn(
                    new DataField<int>(nameof(AuditRequestModel.Work.SourceDb)),
                    records.Select(x => x.Work.SourceDb).ToArray()),
                new DataColumn(
                    new DataField<string>("OriginalTitle"),
                    records.Select(x => x.Work.Titles.FirstOrDefault(x=>x.Type == TitleType.OT)?.Name).ToArray()),
                new DataColumn(
                    new DataField<string>(nameof(AuditRequestModel.TransactionSource)),
                    records.Select(x => x.Work.AdditionalIdentifiers != null && x.Work.AdditionalIdentifiers.Any() ? TransactionSource.Publisher.ToFriendlyString()
                    : TransactionSource.Agency.ToFriendlyString()).ToArray()),
                new DataColumn(
                    new DataField<string>("CreatorNames"),
                    records.Select(x => x.Work.InterestedParties.Any() ? SerializeCreatorNames(x.Work.InterestedParties) : string.Empty).ToArray()),
                new DataColumn(
                    new DataField<string>("CreatorNameNumbers"),
                    records.Select(x => x.Work.InterestedParties.Any() ? SerializeCreatorNameNumbers(x.Work.InterestedParties) : string.Empty).ToArray()),
                new DataColumn(
                    new DataField<long?>("PublisherNameNumber"),
                    records.Select(x =>  x.Work.AdditionalIdentifiers != null && x.Work.AdditionalIdentifiers.Any() ? x.Work.AdditionalIdentifiers.FirstOrDefault(x => x?.NameNumber != null)?.NameNumber : 0).ToArray()),
                new DataColumn(
                    new DataField<string>("PublisherWorkNumber"),
                    records.Select(x => x.Work.AdditionalIdentifiers != null && x.Work.AdditionalIdentifiers.Any() ? x.Work.AdditionalIdentifiers.FirstOrDefault(x => x?.NameNumber != null)?.WorkCode  : string.Empty).ToArray()),
                new DataColumn(
                    new DataField<bool>(nameof(AuditRequestModel.RelatedSubmissionIncludedIswc)),
                    records.Select(x => x.RelatedSubmissionIncludedIswc).ToArray()),
                 new DataColumn(
                    new DataField<string>("AgentVersion"),
                    records.Select(x => string.IsNullOrWhiteSpace(x.AgentVersion) ? x.AgentVersion : string.Empty).ToArray())


            };
        }

        public ParquetSchema Schema => new ParquetSchema(Fields);
        public IReadOnlyCollection<Field> Fields => Columns.Select(x => x.Field).ToList().AsReadOnly();
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
