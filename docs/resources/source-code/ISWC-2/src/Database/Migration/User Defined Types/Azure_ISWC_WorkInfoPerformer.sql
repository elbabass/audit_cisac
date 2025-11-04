CREATE TYPE [Migration].[Azure_ISWC_WorkInfoPerformer] AS TABLE (
    [WorkInfoID]       BIGINT        NOT NULL,
    [PerformerID]      BIGINT        NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL);

