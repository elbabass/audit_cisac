CREATE TYPE [Migration].[Azure_ISWC_ISWC] AS TABLE (
    [IswcID]           BIGINT        NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL,
    [Iswc]             NVARCHAR (11) NOT NULL,
    [AgencyID]         CHAR (3)      NOT NULL);

