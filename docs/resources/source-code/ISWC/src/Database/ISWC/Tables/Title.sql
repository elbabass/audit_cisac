CREATE TABLE [ISWC].[Title] (
    [TitleID]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [Status]             BIT            NOT NULL,
    [Concurrency]        ROWVERSION     NOT NULL,
    [CreatedDate]        DATETIME2 (0)  NOT NULL,
    [LastModifiedDate]   DATETIME2 (0)  NOT NULL,
    [LastModifiedUserID] INT            NOT NULL,
    [IswcID]             BIGINT         NOT NULL,
    [WorkInfoID]         BIGINT         NOT NULL,
    [StandardizedTitle]  NVARCHAR (512) NOT NULL,
    [Title]              NVARCHAR (512) NOT NULL,
    [TitleTypeID]        INT            NOT NULL,
    CONSTRAINT [PK_Title] PRIMARY KEY NONCLUSTERED ([TitleID] ASC) ON [PRIMARY],
    CONSTRAINT [FK_Title_ISWC_IswcID] FOREIGN KEY ([IswcID]) REFERENCES [ISWC].[ISWC] ([IswcID]),
    CONSTRAINT [FK_Title_TitleType_TitleTypeID] FOREIGN KEY ([TitleTypeID]) REFERENCES [Lookup].[TitleType] ([TitleTypeID]),
    CONSTRAINT [FK_Title_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_Title_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
) ON [part_scheme_PartitionByISWCID] ([IswcID]);












GO





GO






GO
CREATE CLUSTERED INDEX [ClusteredIndex_on_Title_IswcID] ON [ISWC].[Title]
(
    [IswcID]
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [part_scheme_PartitionByISWCID]([IswcID])


GO



GO



GO



GO
CREATE NONCLUSTERED INDEX [IX_Title_WorkInfoID]
    ON [ISWC].[Title]([WorkInfoID] ASC)
    INCLUDE([TitleID], [TitleTypeID])
    ON [part_scheme_PartitionByISWCID] ([IswcID]);








GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Title unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'TitleID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Iswc identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'IswcID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work info identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'WorkInfoID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Standardized title', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'StandardizedTitle';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Raw title', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'Title';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Title Type identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Title', @level2type = N'COLUMN', @level2name = N'TitleTypeID';


GO
CREATE NONCLUSTERED INDEX [IX_Title_WorkInfoID_TitleTypeID]
    ON [ISWC].[Title]([WorkInfoID] ASC, [TitleTypeID] ASC)
    INCLUDE([TitleID])
    ON [part_scheme_PartitionByISWCID] ([IswcID]);

GO 
CREATE NONCLUSTERED INDEX [IX_Title_LastModifiedDate]
    ON [ISWC].[Title]([LastModifiedDate])
    INCLUDE([WorkInfoID])
	ON [part_scheme_PartitionByISWCID] ([IswcID]);
GO
CREATE NONCLUSTERED INDEX [IX_Title_Status]
    ON [ISWC].[Title]([Status] ASC, [WorkInfoID] ASC)
    INCLUDE([Title], [TitleTypeID])
    ON [part_scheme_PartitionByISWCID] ([IswcID]);


GO
CREATE NONCLUSTERED INDEX [IX_Title_WorkInfoID_NonPartitioned]
    ON [ISWC].[Title]([WorkInfoID] ASC)
    INCLUDE([TitleID], [TitleTypeID]) WITH (FILLFACTOR = 100)
    ON [PRIMARY];

