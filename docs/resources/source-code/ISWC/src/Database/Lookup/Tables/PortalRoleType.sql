CREATE TABLE [Lookup].[PortalRoleType] (
    [PortalRoleTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Code]             NVARCHAR (20) NOT NULL,
    [Description]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_PortalRoleType] PRIMARY KEY CLUSTERED ([PortalRoleTypeID] ASC)
);

