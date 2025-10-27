CREATE TYPE [Migration].[Azure_Lookup_Agency] AS TABLE (
    [AgencyID]         CHAR (3)      NOT NULL,
    [Name]             NVARCHAR (50) NOT NULL,
    [Country]          NVARCHAR (50) NULL,
    [CreatedDate]      DATETIME2 (0) NOT NULL,
    [LastModifiedDate] DATETIME2 (0) NOT NULL,
    [LastModifiedUser] VARCHAR (20)  NOT NULL);

