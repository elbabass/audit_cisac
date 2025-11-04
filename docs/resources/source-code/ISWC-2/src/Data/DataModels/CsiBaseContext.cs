using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class CsiBaseContext : DbContext
    {
        public CsiBaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<AdditionalIdentifier> AdditionalIdentifier { get; set; }
        public virtual DbSet<Agency> Agency { get; set; }
        public virtual DbSet<AgentVersion> AgentVersion { get; set; }
        public virtual DbSet<Agreement> Agreement { get; set; }
        public virtual DbSet<CisacRoleMapping> CisacRoleMapping { get; set; }
        public virtual DbSet<CommandLog> CommandLog { get; set; }
        public virtual DbSet<Creator> Creator { get; set; }
        public virtual DbSet<CsiAuditReqWork> CsiAuditReqWork { get; set; }
        public virtual DbSet<DeMergeRequest> DeMergeRequest { get; set; }
        public virtual DbSet<DerivedFrom> DerivedFrom { get; set; }
        public virtual DbSet<DerivedWorkType> DerivedWorkType { get; set; }
        public virtual DbSet<DisambiguationIswc> DisambiguationIswc { get; set; }
        public virtual DbSet<DisambiguationReason> DisambiguationReason { get; set; }
        public virtual DbSet<HighWatermark> HighWatermark { get; set; }
        public virtual DbSet<IndexContributors> IndexContributors { get; set; }
        public virtual DbSet<IndexWorkNamesContributorsPerformersCreators> IndexWorkNamesContributorsPerformersCreators { get; set; }
        public virtual DbSet<IndexWorkNumbers> IndexWorkNumbers { get; set; }
        public virtual DbSet<IndexWorkNumbersArchived> IndexWorkNumbersArchived { get; set; }
        public virtual DbSet<Instrumentation> Instrumentation { get; set; }
        public virtual DbSet<InterestedParty> InterestedParty { get; set; }
        public virtual DbSet<IpnameUsage> IpnameUsage { get; set; }
        public virtual DbSet<Iswc> Iswc { get; set; }
        public virtual DbSet<IswclinkedTo> IswclinkedTo { get; set; }
        public virtual DbSet<IswcStatus> IswcStatus { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<MatchType> MatchType { get; set; }
        public virtual DbSet<MergeRequest> MergeRequest { get; set; }
        public virtual DbSet<MergeStatus> MergeStatus { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Name> Name { get; set; }
        public virtual DbSet<NameReference> NameReference { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<NotificationType> NotificationType { get; set; }
        public virtual DbSet<NumberType> NumberType { get; set; }
        public virtual DbSet<Performer> Performer { get; set; }
        public virtual DbSet<PerformerDesignation> PerformerDesignation { get; set; }
        public virtual DbSet<PortalRoleType> PortalRoleType { get; set; }
        public virtual DbSet<Publisher> Publisher { get; set; }
        public virtual DbSet<PublisherSubmitterCode> PublisherSubmitterCode { get; set; }
        public virtual DbSet<Recording> Recording { get; set; }
        public virtual DbSet<RecordingArtist> RecordingArtist { get; set; }
        public virtual DbSet<RoleType> RoleType { get; set; }
        public virtual DbSet<StandardizedTitle> StandardizedTitle { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<SubmissionSource> SubmissionSource { get; set; }
        public virtual DbSet<Title> Title { get; set; }
        public virtual DbSet<TitleType> TitleType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<WebUser> WebUser { get; set; }
        public virtual DbSet<WebUserRole> WebUserRole { get; set; }
        public virtual DbSet<WorkInfo> WorkInfo { get; set; }
        public virtual DbSet<WorkInfoInstrumentation> WorkInfoInstrumentation { get; set; }
        public virtual DbSet<WorkInfoPerformer> WorkInfoPerformer { get; set; }
        public virtual DbSet<WorkflowInstance> WorkflowInstance { get; set; }
        public virtual DbSet<WorkflowStatus> WorkflowStatus { get; set; }
        public virtual DbSet<WorkflowTask> WorkflowTask { get; set; }
        public virtual DbSet<WorkflowTaskStatus> WorkflowTaskStatus { get; set; }
        public virtual DbSet<WorkflowType> WorkflowType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdditionalIdentifier>(entity =>
            {
                entity.ToTable("AdditionalIdentifier", "ISWC");

                entity.Property(e => e.AdditionalIdentifierId).HasColumnName("AdditionalIdentifierID");

                entity.Property(e => e.NumberTypeId).HasColumnName("NumberTypeID");

                entity.Property(e => e.WorkIdentifier).HasMaxLength(20);

                entity.Property(e => e.WorkInfoId).HasColumnName("WorkInfoID");

                entity.HasOne(d => d.NumberType)
                    .WithMany(p => p.AdditionalIdentifier)
                    .HasForeignKey(d => d.NumberTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdditionalIdentifier_NumberType");

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.AdditionalIdentifier)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdditionalIdentifier_WorkInfo");
            });

            modelBuilder.Entity<Agency>(entity =>
            {
                entity.ToTable("Agency", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.AgencyId)
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Agency identifier");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .HasComment("Country");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.DisallowDisambiguateOverwrite)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Disallow overwrite of disambiguation data from these sources");

                entity.Property(e => e.Iswcstatus)
                    .HasColumnName("ISWCStatus")
                    .HasComment("0 for NoISWCAccess, 1 for ISWCAccess");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Agency name");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Agency)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AgentVersion>(entity =>
            {
                entity.ToTable("AgentVersion", "Configuration");

                entity.Property(e => e.AgentVersionId).HasColumnName("AgentVersionID");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.ToTable("Agreement", "IPI");

                entity.HasIndex(e => e.AgencyId);

                entity.HasIndex(e => new { e.AgencyId, e.ToDate, e.FromDate, e.CreationClass, e.EconomicRights, e.IpbaseNumber })
                    .HasDatabaseName("IX_Agreement_IPBaseNumber");

                entity.Property(e => e.AgreementId)
                    .HasColumnName("AgreementID")
                    .HasComment("Membership agreements identifier");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Membership agency");

                entity.Property(e => e.AmendedDateTime)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Amended date/time");

                entity.Property(e => e.CreationClass)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Creation class");

                entity.Property(e => e.EconomicRights)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Economic rights");

                entity.Property(e => e.FromDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time of commencement");

                entity.Property(e => e.IpbaseNumber)
                    .IsRequired()
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13)
                    .HasComment("Interested Party base number reference");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Role within the creation class");

                entity.Property(e => e.SharePercentage)
                    .HasColumnType("decimal(5, 2)")
                    .HasComment("Share percentage");

                entity.Property(e => e.SignedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Agreement date/time of sign off");

                entity.Property(e => e.ToDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time of termination");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Agreement)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IpbaseNumberNavigation)
                    .WithMany(p => p.Agreement)
                    .HasForeignKey(d => d.IpbaseNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CisacRoleMapping>(entity =>
            {
                entity.ToTable("CisacRoleMapping", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.HasIndex(e => e.RoleTypeId);

                entity.Property(e => e.CisacRoleMappingId)
                    .HasColumnName("CisacRoleMappingID")
                    .HasComment("CISAC Role Mapping ID");

                entity.Property(e => e.CisacRoleType)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("CISAC role type");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.Property(e => e.RoleTypeId)
                    .HasColumnName("RoleTypeID")
                    .HasComment("Internal CSI role type");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.CisacRoleMapping)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RoleType)
                    .WithMany(p => p.CisacRoleMapping)
                    .HasForeignKey(d => d.RoleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CommandLog>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Command).IsRequired();

                entity.Property(e => e.CommandType)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.DatabaseName).HasMaxLength(128);

                entity.Property(e => e.ExtendedInfo).HasColumnType("xml");

                entity.Property(e => e.IndexName).HasMaxLength(128);

                entity.Property(e => e.ObjectName).HasMaxLength(128);

                entity.Property(e => e.ObjectType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SchemaName).HasMaxLength(128);

                entity.Property(e => e.StatisticsName).HasMaxLength(128);
            });

            modelBuilder.Entity<Creator>(entity =>
            {
                entity.HasKey(e => e.CreatorId)
                    .IsClustered(false);

                entity.ToTable("Creator", "ISWC");

                entity.HasIndex(e => e.IswcId)
                    .HasDatabaseName("ClusteredIndex_on_Creator_IswcID")
                    .IsClustered();

                entity.HasIndex(e => new { e.IswcId, e.WorkInfoId })
                    .HasDatabaseName("IX_Creator_WorkInfo_WorkInfoID");

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.IpbaseNumber)
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13);

                entity.Property(e => e.IpnameNumber).HasColumnName("IPNameNumber");

                entity.Property(e => e.IswcId).HasColumnName("IswcID");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50);

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(200);

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.LastModifiedUserId).HasColumnName("LastModifiedUserID");

                entity.Property(e => e.RoleTypeId).HasColumnName("RoleTypeID");

                entity.Property(e => e.WorkInfoId).HasColumnName("WorkInfoID");

                entity.HasOne(d => d.IpbaseNumberNavigation)
                    .WithMany(p => p.Creator)
                    .HasForeignKey(d => d.IpbaseNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IpnameNumberNavigation)
                    .WithMany(p => p.Creator)
                    .HasForeignKey(d => d.IpnameNumber);

                entity.HasOne(d => d.Iswc)
                    .WithMany(p => p.Creator)
                    .HasForeignKey(d => d.IswcId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Creator)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RoleType)
                    .WithMany(p => p.Creator)
                    .HasForeignKey(d => d.RoleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.Creator)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CsiAuditReqWork>(entity =>
            {
                entity.HasKey(e => e.ReqTxId)
                    .IsClustered(false);

                entity.ToTable("csi_audit_req_work", "ISWC");

                entity.HasIndex(e => e.ReqTxId)
                    .HasDatabaseName("tmp_nonclustered_nonunique_req_tx_id");

                entity.HasIndex(e => new { e.ReqTxId, e.CreatedDt })
                    .HasDatabaseName("idx_created_dt");

                entity.HasIndex(e => new { e.Soccde, e.Socwrkcde, e.Srcdb })
                    .HasDatabaseName("ClusteredIndex_on_csi_audit_req_work")
                    .IsClustered();

                entity.Property(e => e.ReqTxId)
                    .HasColumnName("req_tx_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ArchIswc)
                    .HasColumnName("arch_iswc")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Cat)
                    .HasColumnName("cat")
                    .HasMaxLength(55)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnName("created_dt")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.Deleted)
                    .HasColumnName("deleted")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Ips).HasColumnName("ips");

                entity.Property(e => e.LastUpdatedDt)
                    .HasColumnName("last_updated_dt")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.LastUpdatedUser)
                    .HasColumnName("last_updated_user")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LstUpdatedDt)
                    .HasColumnName("lst_updated_dt")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.PostedDt)
                    .HasColumnName("posted_dt")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.PrefIswc)
                    .HasColumnName("pref_iswc")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Soccde).HasColumnName("soccde");

                entity.Property(e => e.Socwrkcde)
                    .HasColumnName("socwrkcde")
                    .HasMaxLength(55)
                    .IsUnicode(false);

                entity.Property(e => e.Srcdb).HasColumnName("srcdb");

                entity.Property(e => e.Titles).HasColumnName("titles");

                entity.Property(e => e.UpdateSeq).HasColumnName("update_seq");
            });

            modelBuilder.Entity<DeMergeRequest>(entity =>
            {
                entity.ToTable("DeMergeRequest", "ISWC");

                entity.HasIndex(e => e.AgencyId);

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.DeMergeRequestId)
                    .HasColumnName("DeMergeRequestID")
                    .HasComment("Unique autogenerated ID for the demerge request");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("CISAC agency code reference of the agency who made the request");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.DeMergeStatus).HasComment("0 (Pending), 1 (Complete)");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.DeMergeRequest)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.DeMergeStatusNavigation)
                    .WithMany(p => p.DeMergeRequest)
                    .HasForeignKey(d => d.DeMergeStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DeMergeRequest_MergeStatus_MergeStatusID");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.DeMergeRequest)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DerivedFrom>(entity =>
            {
                entity.ToTable("DerivedFrom", "ISWC");

                entity.HasIndex(e => e.WorkInfoId);

                entity.Property(e => e.DerivedFromId)
                    .HasColumnName("DerivedFromID")
                    .HasComment("Derived from unique identifier reference");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Iswc)
                    .HasMaxLength(11)
                    .HasComment("Derived from work ISWC");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.Property(e => e.Title)
                    .HasMaxLength(512)
                    .HasComment("Derived from work title");

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work info unique identifier reference");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.DerivedFrom)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.DerivedFrom)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DerivedWorkType>(entity =>
            {
                entity.ToTable("DerivedWorkType", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.DerivedWorkTypeId)
                    .HasColumnName("DerivedWorkTypeID")
                    .HasComment("Derived Work Type identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Derived Work description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.DerivedWorkType)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DisambiguationIswc>(entity =>
            {
                entity.ToTable("DisambiguationISWC", "ISWC");

                entity.HasIndex(e => e.WorkInfoId);

                entity.Property(e => e.DisambiguationIswcId)
                    .HasColumnName("DisambiguationIswcID")
                    .HasComment("Disambiguation ISWC unique identifier");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Iswc)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasComment("Disambiguation ISWC");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work info identifier reference");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.DisambiguationIswc)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.DisambiguationIswc)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DisambiguationReason>(entity =>
            {
                entity.ToTable("DisambiguationReason", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.DisambiguationReasonId)
                    .HasColumnName("DisambiguationReasonID")
                    .HasComment("Disambiguation reason identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("Disambiguation Reason description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.DisambiguationReason)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<HighWatermark>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK_HWM_Name");

                entity.ToTable("HighWatermark", "Configuration");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Value).HasColumnType("datetime");
            });

            modelBuilder.Entity<IndexContributors>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Index_Contributors", "Azure");

                entity.Property(e => e.Concurrency).HasColumnType("datetime2(0)");

                entity.Property(e => e.FirstName).HasMaxLength(45);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(136);

                entity.Property(e => e.GeneratedId)
                    .HasColumnName("GeneratedID")
                    .HasMaxLength(43);

                entity.Property(e => e.IpbaseNumber)
                    .IsRequired()
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13);

                entity.Property(e => e.Ipcode).HasColumnName("IPCode");

                entity.Property(e => e.Ipinumber).HasColumnName("IPINumber");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(90);

                entity.Property(e => e.LegalEntityType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<IndexWorkNamesContributorsPerformersCreators>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Index_WorkNamesContributorsPerformers_Creators", "Azure");

                entity.Property(e => e.Concurrency).HasColumnType("datetime2(0)");

                entity.Property(e => e.GeneratedId)
                    .IsRequired()
                    .HasColumnName("GeneratedID")
                    .HasMaxLength(90)
                    .IsUnicode(false);

                entity.Property(e => e.IpibaseNumber)
                    .IsRequired()
                    .HasColumnName("IPIBaseNumber")
                    .HasMaxLength(13);

                entity.Property(e => e.IpicreatedDate)
                    .HasColumnName("IPICreatedDate")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.Ipinumber).HasColumnName("IPINumber");

                entity.Property(e => e.LastName).HasMaxLength(90);

                entity.Property(e => e.PersonFullName).HasMaxLength(136);

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.RoleType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.WorkId).HasColumnName("WorkID");

                entity.Property(e => e.WorkName)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<IndexWorkNumbers>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Index_WorkNumbers", "Azure");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .IsRowVersion();

                entity.Property(e => e.GeneratedId)
                    .IsRequired()
                    .HasColumnName("GeneratedID")
                    .HasMaxLength(1035)
                    .IsUnicode(false);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TypeCode)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.WorkId).HasColumnName("WorkID");
            });

            modelBuilder.Entity<IndexWorkNumbersArchived>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Index_WorkNumbers_Archived", "Azure");

                entity.Property(e => e.GeneratedId)
                    .IsRequired()
                    .HasColumnName("GeneratedID")
                    .HasMaxLength(1035)
                    .IsUnicode(false);

                entity.Property(e => e.Number).HasMaxLength(11);

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.WorkId).HasColumnName("WorkID");
            });

            modelBuilder.Entity<Instrumentation>(entity =>
            {
                entity.ToTable("Instrumentation", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.InstrumentationId)
                    .HasColumnName("InstrumentationID")
                    .HasComment("Instrumentation identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Instrumentation type code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Family)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Instrument family");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Instrument name");

                entity.Property(e => e.Note)
                    .HasMaxLength(450)
                    .HasComment("Additional notes for instrument");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Instrumentation)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<InterestedParty>(entity =>
            {
                entity.HasKey(e => e.IpbaseNumber);

                entity.ToTable("InterestedParty", "IPI");

                entity.HasIndex(e => e.AgencyId);

                entity.HasIndex(e => e.AmendedDateTime)
                    .HasDatabaseName("nci_wi_InterestedParty_81503BCDFBE664900A64574AF0F03B94");

                entity.Property(e => e.IpbaseNumber)
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13)
                    .HasComment("Interested Party base number");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Interested Party agency");

                entity.Property(e => e.AmendedDateTime)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Amended date/time");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Birth date");

                entity.Property(e => e.BirthPlace)
                    .HasMaxLength(30)
                    .HasComment("Birth place");

                entity.Property(e => e.BirthState)
                    .HasMaxLength(30)
                    .HasComment("Birth state");

                entity.Property(e => e.DeathDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Death date");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Gender");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Interested Party type");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.InterestedParty)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IpnameUsage>(entity =>
            {
                entity.HasKey(e => new { e.IpnameNumber, e.Role, e.CreationClass, e.IpbaseNumber });

                entity.ToTable("IPNameUsage", "IPI");

                entity.HasIndex(e => e.IpbaseNumber);

                entity.Property(e => e.IpnameNumber)
                    .HasColumnName("IPNameNumber")
                    .HasComment("Interested Party name number reference");

                entity.Property(e => e.Role)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Role within the creation class");

                entity.Property(e => e.CreationClass)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Creation class");

                entity.Property(e => e.IpbaseNumber)
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13)
                    .HasComment("Interested Party base number reference");

                entity.HasOne(d => d.IpbaseNumberNavigation)
                    .WithMany(p => p.IpnameUsage)
                    .HasForeignKey(d => d.IpbaseNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IpnameNumberNavigation)
                    .WithMany(p => p.IpnameUsage)
                    .HasForeignKey(d => d.IpnameNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Iswc>(entity =>
            {
                entity.ToTable("ISWC", "ISWC");

                entity.HasIndex(e => new { e.IswcId, e.AgencyId })
                    .HasDatabaseName("IX_ISWC_AgencyID");

                entity.HasIndex(e => new { e.IswcId, e.Iswc1 })
                    .HasDatabaseName("IX_ISWC_Iswc");

                entity.Property(e => e.IswcId)
                    .HasColumnName("IswcID")
                    .HasComment("ISWC unique identifier");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("CISAC agency code reference");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Iswc1)
                    .IsRequired()
                    .HasColumnName("Iswc")
                    .HasMaxLength(11)
                    .HasComment("ISWC code");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Iswc)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Iswc)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IswcStatus)
                    .WithMany(p => p.Iswc)
                    .HasForeignKey(d => d.IswcStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IswclinkedTo>(entity =>
            {
                entity.ToTable("ISWCLinkedTo", "ISWC");

                entity.HasIndex(e => e.IswcId);

                entity.HasIndex(e => new { e.Concurrency, e.CreatedDate, e.IswcId, e.LastModifiedDate, e.LastModifiedUserId, e.LinkedToIswc, e.MergeRequest, e.Status, e.DeMergeRequest })
                    .HasDatabaseName("nci_wi_ISWCLinkedTo_AD302FE447EDA7E83A1A4103632B5924");

                entity.Property(e => e.IswcLinkedToId)
                    .HasColumnName("IswcLinkedToID")
                    .HasComment("ISWC Linked To unique identifier");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.IswcId)
                    .HasColumnName("IswcID")
                    .HasComment("ISWC identifier reference");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.LinkedToIswc)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasComment("ISWC code of the ISWC being linked to (i.e. the destination ISWC)");

                entity.Property(e => e.MergeRequest).HasComment("Foreign Key to MergeRequest field");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.DeMergeRequestNavigation)
                    .WithMany(p => p.IswclinkedTo)
                    .HasForeignKey(d => d.DeMergeRequest)
                    .HasConstraintName("FK_ISWCLinkedTo_DeMergeRequest_DeMergeRequestID");

                entity.HasOne(d => d.Iswc)
                    .WithMany(p => p.IswclinkedTo)
                    .HasForeignKey(d => d.IswcId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.IswclinkedTo)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.MergeRequestNavigation)
                    .WithMany(p => p.IswclinkedTo)
                    .HasForeignKey(d => d.MergeRequest)
                    .HasConstraintName("FK_ISWCLinkedTo_MergeRequest_MergeRequestID");
            });

            modelBuilder.Entity<IswcStatus>(entity =>
            {
                entity.HasKey(e => e.IswcStatusId);

                entity.ToTable("IswcStaus", "Lookup");

                entity.Property(e => e.IswcStatusId).HasColumnName("IswcStatusID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(e => e.LanguageTypeId);

                entity.ToTable("Language", "Lookup");

                entity.Property(e => e.LanguageTypeId).HasColumnName("LanguageTypeID");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.LanguageCode)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<MatchType>(entity =>
            {
                entity.ToTable("MatchType", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.MatchTypeId)
                    .HasColumnName("MatchTypeID")
                    .HasComment("Match Type identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Match Type description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.MatchType)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<MergeRequest>(entity =>
            {
                entity.ToTable("MergeRequest", "ISWC");

                entity.HasIndex(e => e.AgencyId);

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.MergeRequestId)
                    .HasColumnName("MergeRequestID")
                    .HasComment("Unique autogenerated ID for the merge request");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("CISAC agency code reference of the agency who made the request");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.MergeStatus).HasComment("0 (Pending), 1 (Complete)");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.MergeRequest)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.MergeRequest)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.MergeStatusNavigation)
                    .WithMany(p => p.MergeRequest)
                    .HasForeignKey(d => d.MergeStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MergeRequest_MergeStatus_MergeStatusID");
            });

            modelBuilder.Entity<MergeStatus>(entity =>
            {
                entity.ToTable("MergeStatus", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.MergeStatusId)
                    .HasColumnName("MergeStatusID")
                    .HasComment("Merge Status identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Merge Status description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.MergeStatus)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message", "Portal");

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.Header)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.LanguageTypeId).HasColumnName("LanguageTypeID");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.MessageBody)
                    .IsRequired()
                    .HasMaxLength(1500);

                entity.Property(e => e.Portal)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.LanguageType)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.LanguageTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_LanguageType");
            });

            modelBuilder.Entity<Name>(entity =>
            {
                entity.HasKey(e => e.IpnameNumber);

                entity.ToTable("Name", "IPI");

                entity.HasIndex(e => e.AgencyId);

                entity.Property(e => e.IpnameNumber)
                    .HasColumnName("IPNameNumber")
                    .HasComment("Interested Party name number")
                    .ValueGeneratedNever();

                entity.Property(e => e.AgencyId)
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Name entry agency");

                entity.Property(e => e.AmendedDateTime)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Amended date/time");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time the name entry was created");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(45)
                    .HasComment("First Name");

                entity.Property(e => e.ForwardingNameNumber).HasComment("Forwarding name number");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(90)
                    .HasComment("Last Name");

                entity.Property(e => e.TypeCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Type of name entry");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.NameNavigation)
                    .HasForeignKey(d => d.AgencyId);
            });

            modelBuilder.Entity<NameReference>(entity =>
            {
                entity.HasKey(e => new { e.IpnameNumber, e.IpbaseNumber });

                entity.ToTable("NameReference", "IPI");

                entity.HasIndex(e => e.IpbaseNumber);

                entity.HasIndex(e => new { e.AmendedDateTime, e.IpbaseNumber })
                    .HasDatabaseName("nci_wi_NameReference_6A3B4FE74FCC25AA8C7DF567ED2D1D22");

                entity.Property(e => e.IpnameNumber)
                    .HasColumnName("IPNameNumber")
                    .HasComment("Interested Party name number reference");

                entity.Property(e => e.IpbaseNumber)
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13)
                    .HasComment("Interested Party base number reference");

                entity.Property(e => e.AmendedDateTime)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Amended date/time");

                entity.HasOne(d => d.IpbaseNumberNavigation)
                    .WithMany(p => p.NameReference)
                    .HasForeignKey(d => d.IpbaseNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IpnameNumberNavigation)
                    .WithMany(p => p.NameReference)
                    .HasForeignKey(d => d.IpnameNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification", "Portal");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.Message).HasMaxLength(250);

                entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");

                entity.Property(e => e.WebUserRoleId).HasColumnName("WebUserRoleID");

                entity.HasOne(d => d.NotificationType)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.NotificationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_NotificationType");

                entity.HasOne(d => d.WebUserRole)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.WebUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_WebUserRole");
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.ToTable("NotificationType", "Lookup");

                entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(80);
            });

            modelBuilder.Entity<NumberType>(entity =>
            {
                entity.ToTable("NumberType", "Lookup");

                entity.Property(e => e.NumberTypeId).HasColumnName("NumberTypeID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Description).HasMaxLength(80);
            });

            modelBuilder.Entity<Performer>(entity =>
            {
                entity.ToTable("Performer", "ISWC");

                entity.HasIndex(e => new { e.Status, e.LastName, e.FirstName })
                    .HasDatabaseName("IX_Performer_LastNameFirstName");

                entity.Property(e => e.PerformerId)
                    .HasColumnName("PerformerID")
                    .HasComment("Work Info Performer identifier");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.Isni)
                    .HasMaxLength(16)
                    .HasComment("Isni number of the performer");

                entity.Property(e => e.Ipn)
                    .HasComment("Ipn number of the performer");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasComment("First name of performer");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Last name of performer");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Performer)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<PerformerDesignation>(entity =>
            {
                entity.HasKey(e => e.PerformerDesignationId);

                entity.ToTable("PerformerDesignation", "Lookup");

                entity.Property(e => e.PerformerDesignationId).HasColumnName("PerformerDesignationID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<PortalRoleType>(entity =>
            {
                entity.ToTable("PortalRoleType", "Lookup");

                entity.Property(e => e.PortalRoleTypeId).HasColumnName("PortalRoleTypeID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(e => e.PublisherId)
                    .IsClustered(false);

                entity.ToTable("Publisher", "ISWC");

                entity.HasIndex(e => e.IswcId)
                    .HasDatabaseName("ClusteredIndex_on_Publisher_IswcID")
                    .IsClustered();

                entity.HasIndex(e => new { e.IswcId, e.WorkInfoId })
                    .HasDatabaseName("IX_Publisher_WorkInfoID");

                entity.Property(e => e.PublisherId).HasColumnName("PublisherID");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.IpbaseNumber)
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13)
                    .HasComment("Interested Party base number reference");

                entity.Property(e => e.IpnameNumber)
                    .HasColumnName("IPNameNumber")
                    .HasComment("Publisher IP Name Number reference");

                entity.Property(e => e.IswcId)
                    .HasColumnName("IswcID")
                    .HasComment("ISWC identifier reference");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.RoleTypeId)
                    .HasColumnName("RoleTypeID")
                    .HasComment("Publisher Role Type");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50);

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(200);

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work info identifier reference");

                entity.HasOne(d => d.IpbaseNumberNavigation)
                    .WithMany(p => p.Publisher)
                    .HasForeignKey(d => d.IpbaseNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IpnameNumberNavigation)
                    .WithMany(p => p.Publisher)
                    .HasForeignKey(d => d.IpnameNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Iswc)
                    .WithMany(p => p.Publisher)
                    .HasForeignKey(d => d.IswcId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Publisher)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RoleType)
                    .WithMany(p => p.Publisher)
                    .HasForeignKey(d => d.RoleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.Publisher)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PublisherSubmitterCode>(entity =>
            {
                entity.ToTable("PublisherSubmitterCode", "Lookup");

                entity.Property(e => e.PublisherSubmitterCodeId).HasColumnName("PublisherSubmitterCodeID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.IpnameNumber).HasColumnName("IPNameNumber");

                entity.Property(e => e.Publisher)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Recording>(entity =>
            {
                entity.HasKey(e => e.RecordingId);

                entity.ToTable("Recording", "ISWC");

                entity.Property(e => e.RecordingId).HasColumnName("RecordingID");

                entity.Property(e => e.RecordingTitle)
                    .HasMaxLength(512)
                    .HasComment("The main title of the recording");

                entity.Property(e => e.SubTitle)
                    .HasMaxLength(512)
                    .HasComment("The sub title of the recording");

                entity.Property(e => e.LabelName)
                    .HasMaxLength(200)
                    .HasComment("Name of label associated with recording");

                entity.Property(e => e.AdditionalIdentifierId);

                entity.Property(e => e.ReleaseEmbargoDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date which details of the recording will be hidden until");

                entity.HasOne(d => d.AdditionalIdentifier)
                    .WithOne(p => p.Recording)
                    .HasConstraintName("FK_Performer_AdditionalIdentifier")
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(d => d.RecordingArtist)
                    .WithOne(p => p.Recording)
                    .HasForeignKey(d => d.RecordingId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<RecordingArtist>(entity =>
            {
                entity.HasKey(e => new { e.RecordingId, e.PerformerId });

                entity.ToTable("RecordingArtist", "ISWC");

                entity.HasIndex(e => e.PerformerId);

                entity.Property(e => e.RecordingId)
                    .HasColumnName("RecordingID")
                    .HasComment("Recording unique identifier reference");

                entity.Property(e => e.PerformerId)
                    .HasColumnName("PerformerID")
                    .HasComment("Performer unique identifier reference");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.Property(e => e.PerformerDesignationId)
                    .HasColumnName("PerformerDesignationID")
                    .HasComment("Whether performer is a Main Artist or Other");

                entity.HasOne(d => d.PerformerDesignation)
                    .WithMany(p => p.RecordingArtist)
                    .HasForeignKey(d => d.PerformerDesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.RecordingArtist)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Performer)
                    .WithMany(p => p.RecordingArtist)
                    .HasForeignKey(d => d.PerformerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Recording)
                    .WithMany(p => p.RecordingArtist)
                    .HasForeignKey(d => d.RecordingId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RoleType>(entity =>
            {
                entity.ToTable("RoleType", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.RoleTypeId)
                    .HasColumnName("RoleTypeID")
                    .HasComment("Role Type identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Role Type description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.RoleType)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StandardizedTitle>(entity =>
            {
                entity.ToTable("StandardizedTitle", "Configuration");

                entity.Property(e => e.StandardizedTitleId).HasColumnName("StandardizedTitleID");

                entity.Property(e => e.ReplacePattern)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.SearchPattern)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Society)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status", "IPI");

                entity.HasIndex(e => e.IpbaseNumber);

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .HasComment("Status identifier");

                entity.Property(e => e.AmendedDateTime)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Amended date/time");

                entity.Property(e => e.ForwardingBaseNumber)
                    .IsRequired()
                    .HasMaxLength(13)
                    .HasComment("Forwarding base number");

                entity.Property(e => e.FromDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time of commencement");

                entity.Property(e => e.IpbaseNumber)
                    .IsRequired()
                    .HasColumnName("IPBaseNumber")
                    .HasMaxLength(13)
                    .HasComment("Interested Party base number reference");

                entity.Property(e => e.StatusCode).HasComment("Status of base number");

                entity.Property(e => e.ToDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time of termination");

                entity.HasOne(d => d.IpbaseNumberNavigation)
                    .WithMany(p => p.Status)
                    .HasForeignKey(d => d.IpbaseNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SubmissionSource>(entity =>
            {
                entity.ToTable("SubmissionSource", "Configuration");

                entity.Property(e => e.SubmissionSourceId).HasColumnName("SubmissionSourceID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.LastModifiedUserId).HasColumnName("LastModifiedUserID");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.SubmissionSource)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.HasKey(e => e.TitleId)
                    .IsClustered(false);

                entity.ToTable("Title", "ISWC");

                entity.HasIndex(e => e.IswcId)
                    .HasDatabaseName("ClusteredIndex_on_Title_IswcID")
                    .IsClustered();

                entity.HasIndex(e => new { e.TitleId, e.Status, e.Title1, e.TitleTypeId, e.IswcId, e.WorkInfoId })
                    .HasDatabaseName("IX_Title_WorkInfoID");

                entity.Property(e => e.TitleId)
                    .HasColumnName("TitleID")
                    .HasComment("Title unique identifier reference");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.IswcId)
                    .HasColumnName("IswcID")
                    .HasComment("Iswc identifier reference");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.StandardizedTitle)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasComment("Standardized title");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.Property(e => e.Title1)
                    .IsRequired()
                    .HasColumnName("Title")
                    .HasMaxLength(512)
                    .HasComment("Raw title");

                entity.Property(e => e.TitleTypeId)
                    .HasColumnName("TitleTypeID")
                    .HasComment("Title Type identifier reference");

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work info identifier reference");

                entity.HasOne(d => d.Iswc)
                    .WithMany(p => p.Title)
                    .HasForeignKey(d => d.IswcId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.Title)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TitleType)
                    .WithMany(p => p.Title)
                    .HasForeignKey(d => d.TitleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.Title)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TitleType>(entity =>
            {
                entity.ToTable("TitleType", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.TitleTypeId)
                    .HasColumnName("TitleTypeID")
                    .HasComment("Title Type identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Title Type description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.TitleType)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasComment("User identifier");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("User description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasComment("User name");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.InverseLastModifiedUser)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WebUser>(entity =>
            {
                entity.ToTable("WebUser", "Portal");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_WebUser")
                    .IsUnique();

                entity.Property(e => e.WebUserId).HasColumnName("WebUserID");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2(0)");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.WebUser)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WebUser_Agency");
            });

            modelBuilder.Entity<WebUserRole>(entity =>
            {
                entity.ToTable("WebUserRole", "Portal");

                entity.Property(e => e.WebUserRoleId).HasColumnName("WebUserRoleID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.WebUserId).HasColumnName("WebUserID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.WebUserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WebUserRole_PortalRoleType");

                entity.HasOne(d => d.WebUser)
                    .WithMany(p => p.WebUserRole)
                    .HasForeignKey(d => d.WebUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WebUserRole_User");
            });

            modelBuilder.Entity<WorkInfo>(entity =>
            {
                entity.HasKey(e => e.WorkInfoId)
                    .IsClustered(false);

                entity.ToTable("WorkInfo", "ISWC");

                entity.HasIndex(e => e.IswcId)
                    .HasDatabaseName("ClusteredIndex_on_WorkInfo_IswcID")
                    .IsClustered();

                entity.HasIndex(e => new { e.IswcId, e.AgencyId })
                    .HasDatabaseName("IX_WorkInfo_AgencyID");

                entity.HasIndex(e => new { e.IswcId, e.AgencyId, e.AgencyWorkCode })
                    .HasDatabaseName("IX_WorkInfo_AgencyWorkCode");

                entity.HasIndex(e => new { e.IswcId, e.Status, e.ArchivedIswc })
                    .HasDatabaseName("IX_WorkInfo_ArchivedIswc");

                entity.HasIndex(e => new { e.WorkInfoId, e.Concurrency, e.IsReplaced, e.AgencyId, e.AgencyWorkCode, e.Status, e.DerivedWorkTypeId, e.IswcId, e.LastModifiedDate })
                    .HasDatabaseName("IX_WorkInfo_LastModifiedDate");

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work Info identifier");

                entity.Property(e => e.AgencyId)
                    .IsRequired()
                    .HasColumnName("AgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("CISAC agency code reference");

                entity.Property(e => e.AgencyWorkCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasComment("CISAC agency provided Work Code");

                entity.Property(e => e.ArchivedIswc)
                    .HasMaxLength(11)
                    .HasComment("Archived ISWC");

                entity.Property(e => e.Bvltr)
                    .HasColumnName("BVLTR")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Background, Logo, Theme, Visual and Rolled Up Cue if provided");

                entity.Property(e => e.CisnetCreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of creation on CIS-Net");

                entity.Property(e => e.CisnetLastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification on CIS-Net");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.DerivedWorkTypeId)
                    .HasColumnName("DerivedWorkTypeID")
                    .HasComment("Indicates if a work submission is a derived work and if so what type: either a Modified Version, an Excerpt or a Composite");

                entity.Property(e => e.Disambiguation).HasComment("Flag to indicate if disambiguation data was included in the submission");

                entity.Property(e => e.DisambiguationReasonId)
                    .HasColumnName("DisambiguationReasonID")
                    .HasComment("Disambiguation data type");

                entity.Property(e => e.Ipcount)
                    .HasColumnName("IPCount")
                    .HasComment("Number of IPs submitted");

                entity.Property(e => e.IsReplaced).HasComment("Flag indicating this work has been merged");

                entity.Property(e => e.IswcEligible).HasComment("Flag indicating eligibility");

                entity.Property(e => e.IswcId)
                    .HasColumnName("IswcID")
                    .HasComment("ISWC unique identifier reference");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.MatchTypeId)
                    .HasColumnName("MatchTypeID")
                    .HasComment("Matching Rule applied");

                entity.Property(e => e.MwiCategory)
                    .HasMaxLength(5)
                    .HasComment("Work Category from CIS-Net");

                entity.Property(e => e.SourceDatabase).HasComment("Source Database");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.WorkInfo)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.DerivedWorkType)
                    .WithMany(p => p.WorkInfo)
                    .HasForeignKey(d => d.DerivedWorkTypeId);

                entity.HasOne(d => d.DisambiguationReason)
                    .WithMany(p => p.WorkInfo)
                    .HasForeignKey(d => d.DisambiguationReasonId);

                entity.HasOne(d => d.Iswc)
                    .WithMany(p => p.WorkInfo)
                    .HasForeignKey(d => d.IswcId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkInfo)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.MatchType)
                    .WithMany(p => p.WorkInfo)
                    .HasForeignKey(d => d.MatchTypeId);
            });

            modelBuilder.Entity<WorkInfoInstrumentation>(entity =>
            {
                entity.HasKey(e => new { e.WorkInfoId, e.InstrumentationId });

                entity.ToTable("WorkInfoInstrumentation", "ISWC");

                entity.HasIndex(e => e.InstrumentationId);

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work info unique identifier reference");

                entity.Property(e => e.InstrumentationId)
                    .HasColumnName("InstrumentationID")
                    .HasComment("Instrumentation unique identifier reference");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.HasOne(d => d.Instrumentation)
                    .WithMany(p => p.WorkInfoInstrumentation)
                    .HasForeignKey(d => d.InstrumentationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkInfoInstrumentation)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.WorkInfoInstrumentation)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkInfoPerformer>(entity =>
            {
                entity.HasKey(e => new { e.WorkInfoId, e.PerformerId });

                entity.ToTable("WorkInfoPerformer", "ISWC");

                entity.HasIndex(e => e.PerformerId);

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Work info unique identifier reference");

                entity.Property(e => e.PerformerId)
                    .HasColumnName("PerformerID")
                    .HasComment("Performer unique identifier reference");

                entity.Property(e => e.Concurrency)
                    .IsRequired()
                    .HasComment("Row version field")
                    .IsRowVersion();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Status).HasComment("Logically deleted status");

                entity.Property(e => e.PerformerDesignationId)
                    .HasColumnName("PerformerDesignationID")
                    .HasComment("Whether performer is a Main Artist or Other");

                entity.HasOne(d => d.PerformerDesignation)
                    .WithMany(p => p.WorkInfoPerformer)
                    .HasForeignKey(d => d.PerformerDesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkInfoPerformer)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Performer)
                    .WithMany(p => p.WorkInfoPerformer)
                    .HasForeignKey(d => d.PerformerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.WorkInfoPerformer)
                    .HasForeignKey(d => d.WorkInfoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkflowInstance>(entity =>
            {
                entity.ToTable("WorkflowInstance", "ISWC");

                entity.HasIndex(e => new { e.CreatedDate, e.DeMergeRequestId, e.InstanceStatus, e.LastModifiedDate, e.LastModifiedUserId, e.MergeRequestId, e.WorkflowType, e.WorkInfoId, e.IsDeleted })
                    .HasDatabaseName("nci_wi_WorkflowInstance_355F1BD0DB0D003CC8259D512893D6C3");

                entity.HasIndex(e => new { e.WorkflowType, e.MergeRequestId, e.CreatedDate, e.LastModifiedDate, e.LastModifiedUserId, e.InstanceStatus, e.IsDeleted, e.DeMergeRequestId, e.WorkInfoId })
                    .HasDatabaseName("IX_WorkflowInstance_WorkInfoID");

                entity.Property(e => e.WorkflowInstanceId)
                    .HasColumnName("WorkflowInstanceID")
                    .HasComment("Unique auto generated identifier for the workflow instance");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time that the WorkflowInstance was created");

                entity.Property(e => e.DeMergeRequestId).HasColumnName("DeMergeRequestID");

                entity.Property(e => e.InstanceStatus).HasComment("0 (Outstanding), 1 (Approved), 2 (Rejected), 3 (Cancelled)");

                entity.Property(e => e.IsDeleted).HasComment("Logically deleted flag");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time that the WorkflowInstance was last modified");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.MergeRequestId)
                    .HasColumnName("MergeRequestID")
                    .HasComment("Unique ID for related MergeRequest record that contains the requested merge info.  Must be populated if the WorkflowType is 2.");

                entity.Property(e => e.WorkInfoId)
                    .HasColumnName("WorkInfoID")
                    .HasComment("Unique ID for related WorkInfo record that contains the modified data.  Must be populated if the WorkflowType is 1.");

                entity.Property(e => e.WorkflowType).HasComment("1 (Update Approval), 2 (Merge Approval)");

                entity.HasOne(d => d.DeMergeRequest)
                    .WithMany(p => p.WorkflowInstance)
                    .HasForeignKey(d => d.DeMergeRequestId);

                entity.HasOne(d => d.InstanceStatusNavigation)
                    .WithMany(p => p.WorkflowInstance)
                    .HasForeignKey(d => d.InstanceStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowInstance_WorkflowStatus_WorkflowStatusID");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkflowInstance)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.MergeRequest)
                    .WithMany(p => p.WorkflowInstance)
                    .HasForeignKey(d => d.MergeRequestId);

                entity.HasOne(d => d.WorkInfo)
                    .WithMany(p => p.WorkflowInstance)
                    .HasForeignKey(d => d.WorkInfoId);

                entity.HasOne(d => d.WorkflowTypeNavigation)
                    .WithMany(p => p.WorkflowInstance)
                    .HasForeignKey(d => d.WorkflowType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowInstance_WorkflowType_WorkflowTypeID");
            });

            modelBuilder.Entity<WorkflowStatus>(entity =>
            {
                entity.ToTable("WorkflowStatus", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.WorkflowStatusId)
                    .HasColumnName("WorkflowStatusID")
                    .HasComment("Workflow Task Status identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Workflow Status description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkflowStatus)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkflowTask>(entity =>
            {
                entity.ToTable("WorkflowTask", "ISWC");

                entity.HasIndex(e => new { e.WorkflowInstanceId, e.AssignedAgencyId })
                    .HasDatabaseName("nci_wi_WorkflowTask_8785A3A411205D7738BE28A73097F469");

                entity.Property(e => e.WorkflowTaskId)
                    .HasColumnName("WorkflowTaskID")
                    .HasComment("Unique auto generated identifier for the workflow task");

                entity.Property(e => e.AssignedAgencyId)
                    .IsRequired()
                    .HasColumnName("AssignedAgencyID")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("ID of related Agency record for the agency to which the approval task is assigned. ");

                entity.Property(e => e.IsDeleted).HasComment("Logically deleted flag");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date/time that the WorkflowInstance was last modified");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user ");

                entity.Property(e => e.Message).HasMaxLength(200);

                entity.Property(e => e.TaskStatus).HasComment("0 (Outstanding), 1 (Approved), 2 (Rejected), 3 (Cancelled)");

                entity.Property(e => e.WorkflowInstanceId)
                    .HasColumnName("WorkflowInstanceID")
                    .HasComment("Unique ID for related WorkflowInstance record ");

                entity.HasOne(d => d.AssignedAgency)
                    .WithMany(p => p.WorkflowTask)
                    .HasForeignKey(d => d.AssignedAgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowTask_Agency_AgencyID");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkflowTask)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TaskStatusNavigation)
                    .WithMany(p => p.WorkflowTask)
                    .HasForeignKey(d => d.TaskStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowTask_WorkflowTaskStatus_TaskStatusID");

                entity.HasOne(d => d.WorkflowInstance)
                    .WithMany(p => p.WorkflowTask)
                    .HasForeignKey(d => d.WorkflowInstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkflowTaskStatus>(entity =>
            {
                entity.ToTable("WorkflowTaskStatus", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.WorkflowTaskStatusId)
                    .HasColumnName("WorkflowTaskStatusID")
                    .HasComment("Workflow Task Status identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Workflow Task Status description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkflowTaskStatus)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkflowType>(entity =>
            {
                entity.ToTable("WorkflowType", "Lookup");

                entity.HasIndex(e => e.LastModifiedUserId);

                entity.Property(e => e.WorkflowTypeId)
                    .HasColumnName("WorkflowTypeID")
                    .HasComment("Workflow Type identifier");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("Entity instance code");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date that the entity instance was created");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("Workflow Type description");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime2(0)")
                    .HasComment("Date of last modification");

                entity.Property(e => e.LastModifiedUserId)
                    .HasColumnName("LastModifiedUserID")
                    .HasComment("The last modifying user");

                entity.HasOne(d => d.LastModifiedUser)
                    .WithMany(p => p.WorkflowType)
                    .HasForeignKey(d => d.LastModifiedUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
