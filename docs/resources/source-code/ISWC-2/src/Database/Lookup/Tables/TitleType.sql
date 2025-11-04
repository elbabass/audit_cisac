CREATE TABLE [Lookup].[TitleType] (
    [TitleTypeID]        INT           IDENTITY (1, 1) NOT NULL,
    [Code]               CHAR (2)      NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [Description]        NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TitleType] PRIMARY KEY CLUSTERED ([TitleTypeID] ASC),
    CONSTRAINT [FK_TitleType_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_TitleType_LastModifiedUserID]
    ON [Lookup].[TitleType]([LastModifiedUserID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Title Type identifier', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'TitleType', @level2type = N'COLUMN', @level2name = N'TitleTypeID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Entity instance code', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'TitleType', @level2type = N'COLUMN', @level2name = N'Code';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'TitleType', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'TitleType', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'TitleType', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Title Type description', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'TitleType', @level2type = N'COLUMN', @level2name = N'Description';

