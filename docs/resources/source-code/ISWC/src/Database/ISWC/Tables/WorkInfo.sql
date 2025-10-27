CREATE TABLE [ISWC].[WorkInfo] (
    [WorkInfoID]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [Status]                 BIT           NOT NULL,
    [Concurrency]            ROWVERSION    NOT NULL,
    [CreatedDate]            DATETIME2 (0) NOT NULL,
    [LastModifiedDate]       DATETIME2 (0) NOT NULL,
    [LastModifiedUserID]     INT           NOT NULL,
    [IswcID]                 BIGINT        NOT NULL,
    [ArchivedIswc]           NVARCHAR (11) NULL,
    [CisnetLastModifiedDate] DATETIME2 (0) NULL,
    [CisnetCreatedDate]      DATETIME2 (0) NULL,
    [IPCount]                INT           NOT NULL,
    [IsReplaced]             BIT           NOT NULL,
    [IswcEligible]           BIT           NOT NULL,
    [MatchTypeID]            INT           NULL,
    [MwiCategory]            NVARCHAR (5)  NULL,
    [AgencyID]               CHAR (3)      NOT NULL,
    [AgencyWorkCode]         NVARCHAR (20) NOT NULL,
    [SourceDatabase]         INT           NOT NULL,
    [Disambiguation]         BIT           NULL,
    [DisambiguationReasonID] INT           NULL,
    [BVLTR]                  CHAR (1)      NULL,
    [DerivedWorkTypeID]      INT           NULL,
    CONSTRAINT [PK_WorkInfo] PRIMARY KEY NONCLUSTERED ([WorkInfoID] ASC) ON [PRIMARY],
    CONSTRAINT [FK_WorkInfo_Agency_AgencyID] FOREIGN KEY ([AgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID]),
    CONSTRAINT [FK_WorkInfo_DerivedWorkType_DerivedWorkTypeID] FOREIGN KEY ([DerivedWorkTypeID]) REFERENCES [Lookup].[DerivedWorkType] ([DerivedWorkTypeID]),
    CONSTRAINT [FK_WorkInfo_DisambiguationReason_DisambiguationReasonID] FOREIGN KEY ([DisambiguationReasonID]) REFERENCES [Lookup].[DisambiguationReason] ([DisambiguationReasonID]),
    CONSTRAINT [FK_WorkInfo_ISWC_IswcID] FOREIGN KEY ([IswcID]) REFERENCES [ISWC].[ISWC] ([IswcID]),
    CONSTRAINT [FK_WorkInfo_MatchType_MatchTypeID] FOREIGN KEY ([MatchTypeID]) REFERENCES [Lookup].[MatchType] ([MatchTypeID]),
    CONSTRAINT [FK_WorkInfo_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
) ON [part_scheme_PartitionByISWCID] ([IswcID]);



















GO
CREATE CLUSTERED INDEX [ClusteredIndex_on_WorkInfo_IswcID] ON [ISWC].[WorkInfo]
(
    [IswcID]
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [part_scheme_PartitionByISWCID]([IswcID])



GO



GO



GO



GO



GO



GO


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfo_ArchivedIswc]
    ON [ISWC].[WorkInfo]([Status] ASC, [ArchivedIswc] ASC)
    ON [part_scheme_PartitionByISWCID] ([IswcID]);



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work Info identifier', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'WorkInfoID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ISWC unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'IswcID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Archived ISWC', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'ArchivedIswc';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification on CIS-Net', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'CisnetLastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of creation on CIS-Net', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'CisnetCreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Number of IPs submitted', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'IPCount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Flag indicating this work has been merged', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'IsReplaced';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Flag indicating eligibility', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'IswcEligible';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Matching Rule applied', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'MatchTypeID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work Category from CIS-Net', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'MwiCategory';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'CISAC agency code reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'AgencyID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'CISAC agency provided Work Code', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'AgencyWorkCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Source Database', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'SourceDatabase';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Flag to indicate if disambiguation data was included in the submission', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'Disambiguation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Disambiguation data type', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'DisambiguationReasonID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Background, Logo, Theme, Visual and Rolled Up Cue if provided', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'BVLTR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Indicates if a work submission is a derived work and if so what type: either a Modified Version, an Excerpt or a Composite', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfo', @level2type = N'COLUMN', @level2name = N'DerivedWorkTypeID';


GO



GO



GO
CREATE NONCLUSTERED INDEX [IX_WorkInfo_LastModifiedDate]
    ON [ISWC].[WorkInfo]([LastModifiedDate] ASC)
    INCLUDE([WorkInfoID], [Concurrency], [IsReplaced], [AgencyID], [AgencyWorkCode], [Status], [DerivedWorkTypeID])
    ON [part_scheme_PartitionByISWCID] ([IswcID]);




GO



GO
CREATE NONCLUSTERED INDEX [IX_WorkInfo_AgencyID_AgencyWorkCode]
    ON [ISWC].[WorkInfo]([AgencyID] ASC, [AgencyWorkCode] ASC)
    ON [part_scheme_PartitionByISWCID] ([IswcID]);


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfo_IsReplaced]
    ON [ISWC].[WorkInfo]([Status] ASC, [IsReplaced] ASC)
    ON [part_scheme_PartitionByISWCID] ([IswcID]);


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfo_IswcID_AgencyID]
    ON [ISWC].[WorkInfo]([IswcID] ASC, [AgencyID] ASC)
    INCLUDE([WorkInfoID]) WITH (FILLFACTOR = 100)
    ON [part_scheme_PartitionByISWCID] ([IswcID]);


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfo_AgencyWorkCode_AgencyID_Status]
    ON [ISWC].[WorkInfo]([AgencyWorkCode] ASC, [AgencyID] ASC, [Status] ASC)
    ON [PRIMARY];

