CREATE TYPE [Migration].[Azure_ISWC_Publisher] AS TABLE (
    [IPBaseNumber]     NVARCHAR (13) NOT NULL,
    [WorkInfoID]       BIGINT        NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL,
    [RoleTypeCode]     VARCHAR (20)  NOT NULL,
    [IswcID]           BIGINT        NOT NULL,
    [IPNameNumber]     INT           NOT NULL);

