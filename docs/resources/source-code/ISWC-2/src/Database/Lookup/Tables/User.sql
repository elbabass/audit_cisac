CREATE TABLE [Lookup].[User] (
    [UserID]             INT           IDENTITY (1, 1) NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [Name]               NVARCHAR (15) NOT NULL,
    [Description]        NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_User_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_User_LastModifiedUserID]
    ON [Lookup].[User]([LastModifiedUserID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'User identifier', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'UserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'User name', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'User description', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'Description';

