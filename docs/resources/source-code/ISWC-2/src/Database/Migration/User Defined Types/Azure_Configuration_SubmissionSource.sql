CREATE TYPE [Migration].[Azure_Configuration_SubmissionSource] AS TABLE (
    [Code]             CHAR (3)      NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL);

