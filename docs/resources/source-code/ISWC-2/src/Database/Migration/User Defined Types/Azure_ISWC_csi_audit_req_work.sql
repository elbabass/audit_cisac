CREATE TYPE [Migration].[Azure_ISWC_csi_audit_req_work] AS TABLE (
    [titles]            NVARCHAR (MAX) NULL,
    [ips]               NVARCHAR (MAX) NULL,
    [created_dt]        DATETIME2 (0)  NULL,
    [soccde]            INT            NULL,
    [socwrkcde]         VARCHAR (55)   NULL,
    [srcdb]             INT            NULL,
    [update_seq]        INT            NULL,
    [last_updated_dt]   DATETIME2 (0)  NULL,
    [last_updated_user] VARCHAR (200)  NULL,
    [req_tx_id]         INT            NULL,
    [arch_iswc]         VARCHAR (12)   NULL,
    [pref_iswc]         VARCHAR (12)   NULL,
    [cat]               VARCHAR (55)   NULL,
    [deleted]           CHAR (1)       NULL,
    [posted_dt]         DATETIME2 (0)  NULL,
    [lst_updated_dt]    DATETIME2 (0)  NULL);

