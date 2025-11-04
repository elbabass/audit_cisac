CREATE TYPE [Migration].[Azure_IPI_Status] AS TABLE (
    [AmendedDateTime]      DATETIME2 (0) NULL,
    [FromDate]             DATETIME2 (0) NOT NULL,
    [ToDate]               DATETIME2 (0) NOT NULL,
    [ForwardingBaseNumber] NVARCHAR (13) NOT NULL,
    [StatusCode]           INT           NOT NULL,
    [IPBaseNumber]         NVARCHAR (13) NOT NULL);

