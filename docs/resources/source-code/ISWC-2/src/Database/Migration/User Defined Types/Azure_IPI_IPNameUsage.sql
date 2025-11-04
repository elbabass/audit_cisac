CREATE TYPE [Migration].[Azure_IPI_IPNameUsage] AS TABLE (
    [IPNameNumber]  INT           NOT NULL,
    [Role]          CHAR (2)      NULL,
    [CreationClass] CHAR (2)      NULL,
    [IPBaseNumber]  NVARCHAR (13) NOT NULL);

