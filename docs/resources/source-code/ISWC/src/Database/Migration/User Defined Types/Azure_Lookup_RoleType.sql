CREATE TYPE [Migration].[Azure_Lookup_RoleType] AS TABLE (
    [Code]             CHAR (2)      NOT NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL,
    [Description]      NVARCHAR (50) NOT NULL);

