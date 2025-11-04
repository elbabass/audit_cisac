CREATE TABLE [ISWC].[Creator]
(
    [IPBaseNumber]       NVARCHAR (13) NULL,
    [WorkInfoID]         BIGINT        NOT NULL,
    [Status]             BIT           NOT NULL,
    [Concurrency]        ROWVERSION    NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [IsDispute]          BIT           NOT NULL,
    [Authoritative]      BIT           NULL,
    [RoleTypeID]         INT           NOT NULL,
    [IswcID]             BIGINT        NOT NULL,
    [IPNameNumber]       BIGINT        NULL,
    [CreatorID]          INT           IDENTITY (1, 1) NOT NULL,
    [IsExcludedFromIswc] BIT           NOT NULL,
    [FirstName] NVARCHAR (50) NULL,
    [LastName] NVARCHAR (50) NULL,
    [DisplayName] NVARCHAR (200) NULL,
    CONSTRAINT [PK_Creator] PRIMARY KEY NONCLUSTERED ([CreatorID] ASC)
);
GO

ALTER TABLE [ISWC].[Creator]
ADD CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber]
    FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty]([IPBaseNumber]);
GO

ALTER TABLE [ISWC].[Creator] CHECK CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber];
GO

ALTER TABLE [ISWC].[Creator]
ADD CONSTRAINT [FK_Creator_ISWC_IswcID]
    FOREIGN KEY ([IswcID]) REFERENCES [ISWC].[ISWC]([IswcID]);
GO

ALTER TABLE [ISWC].[Creator]
ADD CONSTRAINT [FK_Creator_Name_IPNameNumber]
    FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name]([IPNameNumber]);
GO

ALTER TABLE [ISWC].[Creator]
ADD CONSTRAINT [FK_Creator_RoleType_RoleTypeID]
    FOREIGN KEY ([RoleTypeID]) REFERENCES [Lookup].[RoleType]([RoleTypeID]);
GO

ALTER TABLE [ISWC].[Creator]
ADD CONSTRAINT [FK_Creator_User_LastModifiedUserID]
    FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User]([UserID]);
GO

ALTER TABLE [ISWC].[Creator]
ADD CONSTRAINT [FK_Creator_WorkInfo_WorkInfoID]
    FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo]([WorkInfoID]);
GO

CREATE NONCLUSTERED INDEX [IX_Creator_WorkInfoID_NonPartitioned]
    ON [ISWC].[Creator]([WorkInfoID] ASC)
    INCLUDE([CreatorID], [IPNameNumber]) WITH (FILLFACTOR = 100)
    ON [PRIMARY];
GO
