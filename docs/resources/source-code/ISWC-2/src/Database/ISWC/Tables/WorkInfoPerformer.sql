CREATE TABLE [ISWC].[WorkInfoPerformer] (
    [WorkInfoID]                BIGINT        NOT NULL,
    [PerformerID]               BIGINT        NOT NULL,
    [Status]                    BIT           NOT NULL,
    [Concurrency]               ROWVERSION    NOT NULL,
    [CreatedDate]               DATETIME2 (0) NOT NULL,
    [LastModifiedDate]          DATETIME2 (0) NOT NULL,
    [LastModifiedUserID]        INT           NOT NULL,
    [PerformerDesignationID]    INT           NULL,
    CONSTRAINT [PK_WorkInfoPerformer] PRIMARY KEY CLUSTERED ([WorkInfoID] ASC, [PerformerID] ASC),
    CONSTRAINT [FK_WorkInfoPerformer_Performer_PerformerID] FOREIGN KEY ([PerformerID]) REFERENCES [ISWC].[Performer] ([PerformerID]),
    CONSTRAINT [FK_WorkInfoPerformer_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_WorkInfoPerformer_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID]),
    CONSTRAINT [FK_WorkInfoPerformer_PerformerDesignation_PerformerDesignationID] FOREIGN KEY ([PerformerDesignationID]) REFERENCES [Lookup].[PerformerDesignation] ([PerformerDesignationID]),
);










GO





GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work info unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'WorkInfoID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Performer unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'PerformerID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoPerformer', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfoPerformer_WorkInfoID]
    ON [ISWC].[WorkInfoPerformer]([WorkInfoID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfoPerformer_PerformerID]
    ON [ISWC].[WorkInfoPerformer]([PerformerID] ASC);

