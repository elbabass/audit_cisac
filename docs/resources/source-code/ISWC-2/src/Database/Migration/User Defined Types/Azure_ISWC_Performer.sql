CREATE TYPE [Migration].[Azure_ISWC_Performer] AS TABLE (
    [PerformerID]      BIGINT        NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL,
    [FirstName]        NVARCHAR (50) NULL,
    [LastName]         NVARCHAR (50) NOT NULL);

