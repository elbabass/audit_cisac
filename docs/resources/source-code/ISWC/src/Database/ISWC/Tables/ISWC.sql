CREATE TABLE [ISWC].[ISWC] (
    [IswcID]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [Status]             BIT           NOT NULL,
    [Concurrency]        ROWVERSION    NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [Iswc]               NVARCHAR (11) NOT NULL,
    [AgencyID]           CHAR (3)      NOT NULL,
    [IswcStatusID]       INT           NULL,
    CONSTRAINT [PK_ISWC] PRIMARY KEY CLUSTERED ([IswcID] ASC) ON [part_scheme_PartitionByISWCID] ([IswcID]),
    CONSTRAINT [FK_ISWC_Agency_AgencyID] FOREIGN KEY ([AgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID]),
    CONSTRAINT [FK_ISWC_IswcStatus_IswcStatusID] FOREIGN KEY ([IswcStatusID]) REFERENCES [Lookup].[IswcStatus] ([IswcStatusID]),
    CONSTRAINT [FK_ISWC_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
) ON [part_scheme_PartitionByISWCID] ([IswcID]);









GO




GO
CREATE NONCLUSTERED INDEX [IX_ISWC_AgencyID]
    ON [ISWC].[ISWC]([AgencyID] ASC)
    ON [part_scheme_PartitionByISWCID] ([IswcID]);



GO









GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ISWC unique identifier', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'IswcID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ISWC code', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'Iswc';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'CISAC agency code reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWC', @level2type = N'COLUMN', @level2name = N'AgencyID';


GO
CREATE NONCLUSTERED INDEX [IX_ISWC_Iswc]
    ON [ISWC].[ISWC]([Iswc] ASC)
    INCLUDE([IswcID])
    ON [part_scheme_PartitionByISWCID] ([IswcID]);




GO
CREATE NONCLUSTERED INDEX [IX_ISWC_Iswc_Status_Filtered]
    ON [ISWC].[ISWC]([Iswc] ASC, [Status] ASC) WHERE ([Status]=(1))
    ON [PRIMARY];


GO
CREATE NONCLUSTERED INDEX [IX_ISWC_Iswc_Status]
    ON [ISWC].[ISWC]([Status] ASC) WITH (FILLFACTOR = 100)
    ON [PRIMARY];


GO
CREATE NONCLUSTERED INDEX [IX_ISWC_Iswc_Optimized]
    ON [ISWC].[ISWC]([Iswc] ASC)
    INCLUDE([IswcID], [Status])
    ON [PRIMARY];

