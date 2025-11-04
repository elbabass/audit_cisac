CREATE TYPE [Migration].[Azure_IPI_Name] AS TABLE (
    [IPNameNumber]         INT           NOT NULL,
    [AmendedDateTime]      DATETIME2 (0) NOT NULL,
    [FirstName]            NVARCHAR (45) NULL,
    [LastName]             NVARCHAR (90) NOT NULL,
    [CreatedDate]          DATETIME2 (0) NOT NULL,
    [TypeCode]             CHAR (2)      NOT NULL,
    [ForwardingNameNumber] INT           NULL,
    [AgencyID]             CHAR (3)      NULL);

