CREATE TABLE [ISWC].[Creator] (
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
    CONSTRAINT [PK_Creator] PRIMARY KEY NONCLUSTERED ([CreatorID] ASC) ON [PRIMARY],
    ALTER TABLE [ISWC].[Creator]
    ADD CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber]);


GO
ALTER TABLE [ISWC].[Creator] NOCHECK CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber];

,
    CONSTRAINT [FK_Creator_ISWC_IswcID] FOREIGN KEY ([IswcID]) REFERENCES [ISWC].[ISWC] ([IswcID]),
    ALTER TABLE [ISWC].[Creator]
    ADD CONSTRAINT [FK_Creator_Name_IPNameNumber] FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name] ([IPNameNumber]);

,
    CONSTRAINT [FK_Creator_RoleType_RoleTypeID] FOREIGN KEY ([RoleTypeID]) REFERENCES [Lookup].[RoleType] ([RoleTypeID]),
    CONSTRAINT [FK_Creator_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_Creator_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
) ON 
GO
CREATE NONCLUSTERED INDEX [IX_Creator_WorkInfoID_NonPartitioned]
    ON [ISWC].[Creator]([WorkInfoID] ASC)
    INCLUDE([CreatorID], [IPNameNumber]) WITH (FILLFACTOR = 100)
    ON [PRIMARY];

