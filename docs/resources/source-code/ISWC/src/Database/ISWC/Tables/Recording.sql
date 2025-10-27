CREATE TABLE [ISWC].[Recording] (
    [RecordingID]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [RecordingTitle]         NVARCHAR (512) NOT NULL,
    [SubTitle]               NVARCHAR (512) NULL,
    [LabelName]              NVARCHAR (200) NULL,
    [AdditionalIdentifierID] BIGINT         NOT NULL,
    [ReleaseEmbargoDate]     DATETIME2 (0)  NULL,
    CONSTRAINT [PK_Recording] PRIMARY KEY CLUSTERED ([RecordingID] ASC),
    CONSTRAINT [FK_Performer_AdditionalIdentifier] FOREIGN KEY ([AdditionalIdentifierID]) REFERENCES [ISWC].[AdditionalIdentifier] ([AdditionalIdentifierID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Recording_AdditionalIdentifierID]
    ON [ISWC].[Recording]([AdditionalIdentifierID] ASC) WITH (FILLFACTOR = 100);

