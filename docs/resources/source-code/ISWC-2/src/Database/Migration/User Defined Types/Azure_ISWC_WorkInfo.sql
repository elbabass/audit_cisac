CREATE TYPE [Migration].[Azure_ISWC_WorkInfo] AS TABLE (
    [WorkInfoID]             BIGINT        NOT NULL,
    [CreatedDate]            DATETIME2 (0) NOT NULL,
    [LastModifiedDate]       DATETIME2 (0) NOT NULL,
    [LastModifiedUser]       VARCHAR (20)  NOT NULL,
    [IswcID]                 BIGINT        NOT NULL,
    [ArchivedIswc]           NVARCHAR (11) NULL,
    [CisnetLastModifiedDate] DATETIME2 (0) NULL,
    [CisnetCreatedDate]      DATETIME2 (0) NULL,
    [IPCount]                INT           NOT NULL,
    [IsReplaced]             VARCHAR (20)  NOT NULL,
    [IswcEligible]           VARCHAR (20)  NOT NULL,
    [MatchTypeCode]          VARCHAR (20)  NULL,
    [MwiCategory]            NVARCHAR (5)  NULL,
    [AgencyID]               CHAR (3)      NOT NULL,
    [AgencyWorkCode]         NVARCHAR (20) NOT NULL,
    [SourceDatabase]         INT           NOT NULL);

