CREATE TYPE [Migration].[Azure_IPI_InterestedParty] AS TABLE (
    [IPBaseNumber]    NVARCHAR (13) NOT NULL,
    [AmendedDateTime] DATETIME2 (0) NOT NULL,
    [BirthDate]       DATETIME2 (0) NULL,
    [BirthPlace]      NVARCHAR (30) NULL,
    [BirthState]      NVARCHAR (30) NULL,
    [DeathDate]       DATETIME2 (0) NULL,
    [Gender]          CHAR (1)      NULL,
    [Type]            CHAR (1)      NOT NULL,
    [AgencyID]        CHAR (3)      NOT NULL);

