CREATE TABLE [Lookup].[NotificationType] (
    [NotificationTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Code]               NVARCHAR (20) NOT NULL,
    [Description]        NVARCHAR (80) NULL,
    CONSTRAINT [PK_NotificationType] PRIMARY KEY CLUSTERED ([NotificationTypeID] ASC)
);

