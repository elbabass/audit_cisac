CREATE TABLE [ISWC].[ISWCLinkedTo] (
    [IswcLinkedToID]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [Status]             BIT           NOT NULL,
    [Concurrency]        ROWVERSION    NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [IswcID]             BIGINT        NOT NULL,
    [LinkedToIswc]       NVARCHAR (11) NOT NULL,
    [MergeRequest]       BIGINT        NULL,
    [DeMergeRequest]     BIGINT        NULL,
    CONSTRAINT [PK_ISWCLinkedTo] PRIMARY KEY CLUSTERED ([IswcLinkedToID] ASC),
    CONSTRAINT [FK_ISWCLinkedTo_DeMergeRequest_DeMergeRequestID] FOREIGN KEY ([DeMergeRequest]) REFERENCES [ISWC].[DeMergeRequest] ([DeMergeRequestID]),
    CONSTRAINT [FK_ISWCLinkedTo_ISWC_IswcID] FOREIGN KEY ([IswcID]) REFERENCES [ISWC].[ISWC] ([IswcID]),
    CONSTRAINT [FK_ISWCLinkedTo_MergeRequest_MergeRequestID] FOREIGN KEY ([MergeRequest]) REFERENCES [ISWC].[MergeRequest] ([MergeRequestID]),
    CONSTRAINT [FK_ISWCLinkedTo_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);




GO
CREATE NONCLUSTERED INDEX [IX_ISWCLinkedTo_IswcID]
    ON [ISWC].[ISWCLinkedTo]([IswcID] ASC);


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ISWC Linked To unique identifier', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'IswcLinkedToID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ISWC identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'IswcID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ISWC code of the ISWC being linked to (i.e. the destination ISWC)', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'LinkedToIswc';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Foreign Key to MergeRequest field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'ISWCLinkedTo', @level2type = N'COLUMN', @level2name = N'MergeRequest';
