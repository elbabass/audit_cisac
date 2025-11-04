CREATE TYPE [Migration].[Azure_Lookup_User] AS TABLE (
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL,
    [Name]             NVARCHAR (15) NOT NULL,
    [Description]      NVARCHAR (50) NOT NULL);

