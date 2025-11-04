CREATE TABLE [ISWC].[AdditionalIdentifier] (
    [AdditionalIdentifierID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [WorkInfoID]             BIGINT        NOT NULL,
    [NumberTypeID]           INT           NOT NULL,
    [WorkIdentifier]         NVARCHAR (20) NULL,
    CONSTRAINT [PK_AdditionalIdentifier] PRIMARY KEY CLUSTERED ([AdditionalIdentifierID] ASC),
    CONSTRAINT [FK_AdditionalIdentifier_NumberType] FOREIGN KEY ([NumberTypeID]) REFERENCES [Lookup].[NumberType] ([NumberTypeID]),
    CONSTRAINT [FK_AdditionalIdentifier_WorkInfo] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
);





GO
CREATE NONCLUSTERED INDEX [IX_AdditionalIdentifier_WorkInfoID]
    ON [ISWC].[AdditionalIdentifier]([WorkInfoID] ASC, [NumberTypeID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AdditionalIdentifier_NumberTypeID]
    ON [ISWC].[AdditionalIdentifier]([NumberTypeID] ASC)
    INCLUDE([WorkInfoID], [AdditionalIdentifierID]);

GO

CREATE NONCLUSTERED INDEX [IX_AdditionalIdentifier_WorkIdentifier]
    ON [ISWC].[AdditionalIdentifier]([WorkIdentifier] ASC);
    
GO
