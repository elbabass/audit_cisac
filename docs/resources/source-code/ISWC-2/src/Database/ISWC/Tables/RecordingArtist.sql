CREATE TABLE [ISWC].[RecordingArtist] (
    [RecordingID]               BIGINT        NOT NULL,
    [PerformerID]               BIGINT        NOT NULL,
    [Status]                    BIT           NOT NULL,
    [Concurrency]               ROWVERSION    NOT NULL,
    [CreatedDate]               DATETIME2 (0) NOT NULL,
    [LastModifiedDate]          DATETIME2 (0) NOT NULL,
    [LastModifiedUserID]        INT           NOT NULL,
    [PerformerDesignationID]    INT           NULL,
    CONSTRAINT [PK_RecordingArtist] PRIMARY KEY CLUSTERED ([RecordingID] ASC, [PerformerID] ASC),
    CONSTRAINT [FK_RecordingArtist_Recording_RecordingID] FOREIGN KEY ([RecordingID]) REFERENCES [ISWC].[Recording] ([RecordingID]), 
    CONSTRAINT [FK_RecordingArtist_Performer_PerformerID] FOREIGN KEY ([PerformerID]) REFERENCES [ISWC].[Performer] ([PerformerID]),
    CONSTRAINT [FK_RecordingArtist_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_RecordingArtist_PerformerDesignation_PerformerDesignationID] FOREIGN KEY ([PerformerDesignationID]) REFERENCES [Lookup].[PerformerDesignation] ([PerformerDesignationID]),
);










GO





GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work info unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'RecordingID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Performer unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'PerformerID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'RecordingArtist', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
CREATE NONCLUSTERED INDEX [IX_RecordingArtist_RecordingID]
    ON [ISWC].[RecordingArtist]([RecordingID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RecordingArtist_PerformerID]
    ON [ISWC].[RecordingArtist]([PerformerID] ASC);

