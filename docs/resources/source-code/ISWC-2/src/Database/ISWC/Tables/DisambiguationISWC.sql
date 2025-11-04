CREATE TABLE [ISWC].[DisambiguationISWC] (
    [DisambiguationIswcID] INT           IDENTITY (1, 1) NOT NULL,
    [Status]               BIT           NOT NULL,
    [Concurrency]          ROWVERSION    NOT NULL,
    [CreatedDate]          DATETIME2 (0) NOT NULL,
    [LastModifiedDate]     DATETIME2 (0) NOT NULL,
    [LastModifiedUserID]   INT           NOT NULL,
    [Iswc]                 NVARCHAR (11) NOT NULL,
    [WorkInfoID]           BIGINT        NOT NULL,
    CONSTRAINT [PK_DisambiguationISWC] PRIMARY KEY CLUSTERED ([DisambiguationIswcID] ASC),
    CONSTRAINT [FK_DisambiguationISWC_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_DisambiguationISWC_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
);






GO





GO



GO
CREATE NONCLUSTERED INDEX [IX_DisambiguationISWC_WorkInfoID]
    ON [ISWC].[DisambiguationISWC]([WorkInfoID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Disambiguation ISWC unique identifier', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'DisambiguationIswcID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Disambiguation ISWC', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'Iswc';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work info identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'DisambiguationISWC', @level2type = N'COLUMN', @level2name = N'WorkInfoID';

