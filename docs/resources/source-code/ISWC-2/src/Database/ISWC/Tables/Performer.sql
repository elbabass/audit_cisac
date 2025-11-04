CREATE TABLE [ISWC].[Performer] (
    [PerformerID]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [Status]             BIT           NOT NULL,
    [Concurrency]        ROWVERSION    NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [FirstName]          NVARCHAR (50) NULL,
    [LastName]           NVARCHAR (50) NOT NULL,
    [Isni]               NVARCHAR (16) NULL,
    [Ipn]                INT           NULL,
    CONSTRAINT [PK_Performer] PRIMARY KEY CLUSTERED ([PerformerID] ASC),
    CONSTRAINT [FK_Performer_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);








GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work Info Performer identifier', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'PerformerID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'First name of performer', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'FirstName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Last name of performer', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'Performer', @level2type = N'COLUMN', @level2name = N'LastName';


GO
CREATE NONCLUSTERED INDEX [IX_Performer_LastNameFirstName]
    ON [ISWC].[Performer]([Status] ASC, [LastName] ASC, [FirstName] ASC);

