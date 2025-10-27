CREATE TYPE [Migration].[Azure_ISWC_Title] AS TABLE (
    [TitleID]           BIGINT         NOT NULL,
    [CreatedDate]       DATETIME2 (0)  NOT NULL,
    [LastModifiedDate]  DATETIME2 (0)  NOT NULL,
    [LastModifiedUser]  VARCHAR (20)   NOT NULL,
    [IswcID]            BIGINT         NOT NULL,
    [WorkInfoID]        BIGINT         NOT NULL,
    [StandardizedTitle] NVARCHAR (512) NOT NULL,
    [Title]             NVARCHAR (512) NOT NULL,
    [TitleTypeCode]     NVARCHAR (20)  NOT NULL);

