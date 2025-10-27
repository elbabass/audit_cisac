CREATE TABLE [ISWC].[Publisher] (
    [IPBaseNumber]       NVARCHAR (13)  NULL,
    [WorkInfoID]         BIGINT         NOT NULL,
    [Status]             BIT            NOT NULL,
    [Concurrency]        ROWVERSION     NOT NULL,
    [CreatedDate]        DATETIME2 (0)  NOT NULL,
    [LastModifiedDate]   DATETIME2 (0)  NOT NULL,
    [LastModifiedUserID] INT            NOT NULL,
    [RoleTypeID]         INT            NOT NULL,
    [IswcID]             BIGINT         NOT NULL,
    [IPNameNumber]       BIGINT         NULL,
    [PublisherID]        INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]          NVARCHAR (200) NULL,
    [LastName]           NVARCHAR (200) NULL,
    [DisplayName]        NVARCHAR (200) NULL,
    CONSTRAINT [PK_Publisher] PRIMARY KEY NONCLUSTERED ([PublisherID] ASC) ON [PRIMARY],
    CONSTRAINT [FK_Publisher_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber]) NOT FOR REPLICATION,
    CONSTRAINT [FK_Publisher_ISWC_IswcID] FOREIGN KEY ([IswcID]) REFERENCES [ISWC].[ISWC] ([IswcID]),
    CONSTRAINT [FK_Publisher_Name_IPNameNumber] FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name] ([IPNameNumber]) NOT FOR REPLICATION,
    CONSTRAINT [FK_Publisher_RoleType_RoleTypeID] FOREIGN KEY ([RoleTypeID]) REFERENCES [Lookup].[RoleType] ([RoleTypeID]),
    CONSTRAINT [FK_Publisher_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_Publisher_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
) ON [part_scheme_PartitionByISWCID] ([IswcID]);








GO


CREATE CLUSTERED INDEX [ClusteredIndex_on_Publisher_IswcID]
    ON [ISWC].[Publisher]([IswcID] ASC)
    ON [part_scheme_PartitionByISWCID] ([IswcID]);
GO


EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Interested Party base number reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'IPBaseNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Work info identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'WorkInfoID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Publisher Role Type', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'RoleTypeID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ISWC identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'IswcID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Publisher IP Name Number reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Publisher', @level2type = N'COLUMN', @level2name = N'IPNameNumber';


GO
CREATE NONCLUSTERED INDEX [IX_Publisher_WorkInfoID]
    ON [ISWC].[Publisher]([WorkInfoID] ASC)
    INCLUDE([IPNameNumber], [PublisherID])
    ON [part_scheme_PartitionByISWCID] ([IswcID]);



GO
CREATE NONCLUSTERED INDEX [IX_Publisher_WorkInfoID_NonPartitioned]
    ON [ISWC].[Publisher]([WorkInfoID] ASC)
    INCLUDE([IPNameNumber], [PublisherID]) WITH (FILLFACTOR = 100)
    ON [PRIMARY];

