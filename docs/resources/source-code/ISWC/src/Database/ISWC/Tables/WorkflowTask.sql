CREATE TABLE [ISWC].[WorkflowTask] (
    [WorkflowTaskID]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceID] BIGINT        NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [AssignedAgencyID]   CHAR (3)      NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    [TaskStatus]         INT           NOT NULL,
    [IsDeleted]          BIT           NOT NULL,
    [Message]            NVARCHAR(200) NULL, 
    CONSTRAINT [WorkflowTaskID] PRIMARY KEY CLUSTERED ([WorkflowTaskID] ASC),
    CONSTRAINT [FK_WorkflowTask_Agency_AgencyID] FOREIGN KEY ([AssignedAgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID]),
    CONSTRAINT [FK_WorkflowTask_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_WorkflowTask_WorkflowInstance_WorkflowInstanceID] FOREIGN KEY ([WorkflowInstanceID]) REFERENCES [ISWC].[WorkflowInstance] ([WorkflowInstanceID]),
    CONSTRAINT [FK_WorkflowTask_WorkflowTaskStatus_TaskStatusID] FOREIGN KEY ([TaskStatus]) REFERENCES [Lookup].[WorkflowTaskStatus] ([WorkflowTaskStatusID])
);



GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Unique auto generated identifier for the workflow task', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'WorkflowTaskID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Unique ID for related WorkflowInstance record ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'WorkflowInstanceID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time that the WorkflowInstance was last modified', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'ID of related Agency record for the agency to which the approval task is assigned. ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'AssignedAgencyID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '0 (Outstanding), 1 (Approved), 2 (Rejected), 3 (Cancelled)', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'TaskStatus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted flag', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkflowTask', @level2type = N'COLUMN', @level2name = N'IsDeleted';

