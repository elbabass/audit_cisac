CREATE TABLE [Portal].[WebUserRole] (
    [WebUserRoleID]    INT           IDENTITY (1, 1) NOT NULL,
    [WebUserID]        INT           NOT NULL,
    [RoleID]           INT           NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [Status]           BIT           NOT NULL,
    [IsApproved]       BIT           NOT NULL,
    CONSTRAINT [PK_WebUserRole] PRIMARY KEY CLUSTERED ([WebUserRoleID] ASC),
    CONSTRAINT [FK_WebUserRole_PortalRoleType] FOREIGN KEY ([RoleID]) REFERENCES [Lookup].[PortalRoleType] ([PortalRoleTypeID]),
    CONSTRAINT [FK_WebUserRole_User] FOREIGN KEY ([WebUserID]) REFERENCES [Portal].[WebUser] ([WebUserID])
);



