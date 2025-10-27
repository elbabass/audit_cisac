CREATE TABLE [ISWC].[WorkInfoInstrumentation] (
    [WorkInfoID]         BIGINT        NOT NULL,
    [InstrumentationID]  INT           NOT NULL,
    [Status]             BIT           NOT NULL,
    [Concurrency]        ROWVERSION    NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    CONSTRAINT [PK_WorkInfoInstrumentation] PRIMARY KEY CLUSTERED ([WorkInfoID] ASC, [InstrumentationID] ASC),
    CONSTRAINT [FK_WorkInfoInstrumentation_Instrumentation_InstrumentationID] FOREIGN KEY ([InstrumentationID]) REFERENCES [Lookup].[Instrumentation] ([InstrumentationID]),
    CONSTRAINT [FK_WorkInfoInstrumentation_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID]),
    CONSTRAINT [FK_WorkInfoInstrumentation_WorkInfo_WorkInfoID] FOREIGN KEY ([WorkInfoID]) REFERENCES [ISWC].[WorkInfo] ([WorkInfoID])
);






GO





GO
CREATE NONCLUSTERED INDEX [IX_WorkInfoInstrumentation_InstrumentationID]
    ON [ISWC].[WorkInfoInstrumentation]([InstrumentationID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfoInstrumentation_LastModifiedUserID]
    ON [ISWC].[WorkInfoInstrumentation]([LastModifiedUserID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Work info unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'WorkInfoID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Instrumentation unique identifier reference', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'InstrumentationID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Logically deleted status', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Row version field', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'Concurrency';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user ', @level0type = N'SCHEMA', @level0name = N'ISWC', @level1type = N'TABLE', @level1name = N'WorkInfoInstrumentation', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
CREATE NONCLUSTERED INDEX [IX_WorkInfoInstrumentation_WorkInfoID]
    ON [ISWC].[WorkInfoInstrumentation]([WorkInfoID] ASC);

