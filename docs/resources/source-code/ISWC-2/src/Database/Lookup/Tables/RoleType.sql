CREATE TABLE [Lookup].[RoleType] (
    [RoleTypeID]         INT           IDENTITY (1, 1) NOT NULL,
    [Code]               CHAR (2)      NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [Description]        NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RoleType] PRIMARY KEY CLUSTERED ([RoleTypeID] ASC),
    CONSTRAINT [FK_RoleType_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_RoleType_LastModifiedUserID]
    ON [Lookup].[RoleType]([LastModifiedUserID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Role Type identifier', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'RoleType', @level2type = N'COLUMN', @level2name = N'RoleTypeID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Entity instance code', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'RoleType', @level2type = N'COLUMN', @level2name = N'Code';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'RoleType', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'RoleType', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'RoleType', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Role Type description', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'RoleType', @level2type = N'COLUMN', @level2name = N'Description';

