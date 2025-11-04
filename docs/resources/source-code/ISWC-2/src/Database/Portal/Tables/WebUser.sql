CREATE TABLE [Portal].[WebUser] (
    [WebUserID]        INT            IDENTITY (1, 1) NOT NULL,
    [Email]            NVARCHAR (100) NOT NULL,
    [AgencyID]         CHAR (3)       NOT NULL,
    [Status]           BIT            NOT NULL,
    [CreatedDate]      DATETIME2 (0)  NOT NULL,
    [LastModifiedDate] DATETIME2 (0)  NOT NULL,
    CONSTRAINT [PK_WebUser] PRIMARY KEY CLUSTERED ([WebUserID] ASC),
    CONSTRAINT [FK_WebUser_Agency] FOREIGN KEY ([AgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID]),
    CONSTRAINT [IX_WebUser] UNIQUE NONCLUSTERED ([Email] ASC)
);



