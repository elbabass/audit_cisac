CREATE TYPE [Migration].[Azure_Lookup_TitleTypes] AS TABLE (
    [Code]             VARCHAR (100) NOT NULL,
    [Description]      VARCHAR (100) NOT NULL,
    [CreatedDate]      DATETIME2 (7) NOT NULL,
    [LastModifiedDate] DATETIME2 (7) NOT NULL,
    [LastModifiedUser] VARCHAR (100) NOT NULL);

