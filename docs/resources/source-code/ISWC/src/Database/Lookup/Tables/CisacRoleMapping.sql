CREATE TABLE [Lookup].[CisacRoleMapping] (
    [CisacRoleMappingID] INT           IDENTITY (1, 1) NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [RoleTypeID]         INT           NOT NULL,
    [CisacRoleType]      CHAR (2)      NOT NULL,
    CONSTRAINT [PK_CisacRoleMapping] PRIMARY KEY CLUSTERED ([CisacRoleMappingID] ASC),
    CONSTRAINT [FK_CisacRoleMapping_RoleType_RoleTypeID] FOREIGN KEY ([RoleTypeID]) REFERENCES [Lookup].[RoleType] ([RoleTypeID]),
    CONSTRAINT [FK_CisacRoleMapping_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_CisacRoleMapping_LastModifiedUserID]
    ON [Lookup].[CisacRoleMapping]([LastModifiedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CisacRoleMapping_RoleTypeID]
    ON [Lookup].[CisacRoleMapping]([RoleTypeID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'CISAC Role Mapping ID', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'CisacRoleMapping', @level2type = N'COLUMN', @level2name = N'CisacRoleMappingID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'CisacRoleMapping', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'CisacRoleMapping', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'CisacRoleMapping', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Internal CSI role type', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'CisacRoleMapping', @level2type = N'COLUMN', @level2name = N'RoleTypeID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'CISAC role type', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'CisacRoleMapping', @level2type = N'COLUMN', @level2name = N'CisacRoleType';

