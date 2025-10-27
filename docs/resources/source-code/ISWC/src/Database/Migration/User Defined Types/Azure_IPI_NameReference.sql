CREATE TYPE [Migration].[Azure_IPI_NameReference] AS TABLE (
    [IPNameNumber]    INT           NOT NULL,
    [AmendedDateTime] DATETIME2 (0) NOT NULL,
    [IPBaseNumber]    NVARCHAR (13) NOT NULL);

