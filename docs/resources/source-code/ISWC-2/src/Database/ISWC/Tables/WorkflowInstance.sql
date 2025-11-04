CREATE TABLE [ISWC].[WorkflowInstance] (
    [WorkflowInstanceID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [WorkflowType]       INT           NOT NULL,
    [WorkInfoID]         BIGINT        NULL,
    [MergeRequestID]     BIGINT        NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [InstanceStatus]     INT           NOT NULL,
    [IsDeleted]          BIT           NOT NULL,
    [DeMergeRequestID]   BIGINT        NULL,
    CONSTRAINT [PK_WorkflowInstance] PRIMARY KEY CLUSTERED ([WorkflowInstanceID] ASC),
    CONSTRAINT [FK_WorkflowInstance_DeMergeRequest_DeMergeRequestID] FOREIGN KEY ([DeMergeRequestID]) REFERENCES [ISWC].[DeMergeRequest] ([DeMergeRequestID]),
    CONSTRAINT [FK_WorkflowInstance_MergeRequest_MergeRequestID] FOREIGN KEY ([MergeRequestID]) REFERENCES [ISWC].[MergeRequest] ([MergeRequestID]),
    CONSTRAINT [FK_WorkflowInstance_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_WorkflowInstance_WorkflowStatus_WorkflowStatusID] FOREIGN KEY ([InstanceStatus]) REFERENCES [Lookup].[WorkflowStatus] ([WorkflowStatusID]),
    CONSTRAINT [FK_WorkflowInstance_WorkflowType_WorkflowTypeID] FOREIGN KEY ([WorkflowType]) REFERENCES [Lookup].[WorkflowType] ([WorkflowTypeID]),
    CONSTRAINT [FK_WorkflowInstance_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
);






GO






GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Unique auto generated identifier for the workflow instance', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'WorkflowInstanceID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '1 (Update Approval), 2 (Merge Approval)', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'WorkflowType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Unique ID for related WorkInfo record that contains the modified data.  Must be populated if the WorkflowType is 1.', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'WorkInfoID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Unique ID for related MergeRequest record that contains the requested merge info.  Must be populated if the WorkflowType is 2.', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'MergeRequestID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time that the WorkflowInstance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time that the WorkflowInstance was last modified', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '0 (Outstanding), 1 (Approved), 2 (Rejected), 3 (Cancelled)', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'InstanceStatus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted flag', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowInstance', @level2type = N'COLUMN', @level2name = N'IsDeleted';


GO
CREATE NONCLUSTERED INDEX [IX_WorkflowInstance_WorkInfoID]
    ON [ISWC].[WorkflowInstance]([WorkInfoID] ASC)
    INCLUDE([WorkflowType], [MergeRequestID], [CreatedDate], [LastModifiedDate], [LastModifiedUserID], [InstanceStatus], [IsDeleted], [DeMergeRequestID]);

