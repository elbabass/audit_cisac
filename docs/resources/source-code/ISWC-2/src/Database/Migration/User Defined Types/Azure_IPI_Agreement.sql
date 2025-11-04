CREATE TYPE [Migration].[Azure_IPI_Agreement] AS TABLE (
    [AgreementID]     BIGINT         NOT NULL,
    [AmendedDateTime] DATETIME2 (0)  NOT NULL,
    [CreationClass]   CHAR (2)       NOT NULL,
    [FromDate]        DATETIME2 (0)  NOT NULL,
    [ToDate]          DATETIME2 (0)  NOT NULL,
    [EconomicRights]  CHAR (2)       NOT NULL,
    [Role]            CHAR (2)       NOT NULL,
    [SharePercentage] DECIMAL (5, 2) NOT NULL,
    [SignedDate]      DATETIME2 (0)  NULL,
    [AgencyID]        CHAR (3)       NOT NULL,
    [IPBaseNumber]    NVARCHAR (13)  NOT NULL);

