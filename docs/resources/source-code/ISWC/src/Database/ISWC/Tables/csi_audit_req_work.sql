CREATE TABLE [ISWC].[csi_audit_req_work] (
    [titles]            NVARCHAR (MAX) NULL,
    [ips]               NVARCHAR (MAX) NULL,
    [created_dt]        DATETIME2 (0)  NULL,
    [soccde]            INT            NULL,
    [socwrkcde]         VARCHAR (55)   NULL,
    [srcdb]             INT            NULL,
    [update_seq]        INT            NULL,
    [last_updated_dt]   DATETIME2 (0)  NULL,
    [last_updated_user] VARCHAR (200)  NULL,
    [req_tx_id]         INT            NOT NULL,
    [arch_iswc]         VARCHAR (12)   NULL,
    [pref_iswc]         VARCHAR (12)   NULL,
    [cat]               VARCHAR (55)   NULL,
    [deleted]           CHAR (1)       NULL,
    [posted_dt]         DATETIME2 (0)  NULL,
    [lst_updated_dt]    DATETIME2 (0)  NULL,
    CONSTRAINT [PK_csi_audit_req_work] PRIMARY KEY NONCLUSTERED ([req_tx_id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [tmp_nonclustered_nonunique_req_tx_id]
    ON [ISWC].[csi_audit_req_work]([req_tx_id] ASC);


GO
CREATE CLUSTERED INDEX [ClusteredIndex_on_csi_audit_req_work]
    ON [ISWC].[csi_audit_req_work]([soccde] ASC, [socwrkcde] ASC, [srcdb] ASC);


GO
CREATE NONCLUSTERED INDEX [idx_created_dt]
    ON [ISWC].[csi_audit_req_work]([created_dt] ASC)
    INCLUDE([req_tx_id]);


GO
ALTER INDEX [idx_created_dt]
    ON [ISWC].[csi_audit_req_work] DISABLE;

