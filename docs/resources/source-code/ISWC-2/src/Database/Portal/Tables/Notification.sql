CREATE TABLE [Portal].[Notification] (
    [NotificationID]     INT            IDENTITY (1, 1) NOT NULL,
    [WebUserRoleID]      INT            NOT NULL,
    [Status]             BIT            NOT NULL,
    [Message]            NVARCHAR (250) NULL,
    [NotificationTypeID] INT            NOT NULL,
    [CreatedDate]        DATETIME2 (0)  NOT NULL,
    [LastModifiedDate]   DATETIME2 (0)  NOT NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([NotificationID] ASC),
    CONSTRAINT [FK_Notification_NotificationType] FOREIGN KEY ([NotificationTypeID]) REFERENCES [Lookup].[NotificationType] ([NotificationTypeID]),
    CONSTRAINT [FK_Notification_WebUserRole] FOREIGN KEY ([WebUserRoleID]) REFERENCES [Portal].[WebUserRole] ([WebUserRoleID])
);







