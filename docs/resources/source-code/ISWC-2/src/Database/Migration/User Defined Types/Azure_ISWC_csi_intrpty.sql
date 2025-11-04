CREATE TYPE [Migration].[Azure_ISWC_csi_intrpty] AS TABLE (
    [ip_id]          BIGINT        NULL,
    [ip_base_number] VARCHAR (13)  NULL,
    [created_dt]     DATETIME2 (0) NULL,
    [created_user]   VARCHAR (20)  NULL,
    [update_seq]     INT           NULL);

