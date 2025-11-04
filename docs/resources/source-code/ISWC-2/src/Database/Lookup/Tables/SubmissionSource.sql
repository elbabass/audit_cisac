CREATE TABLE [Lookup].[SubmissionSource]
(
	[SubmissionSourceID] INT           IDENTITY (1, 1) NOT NULL,
	[Code]               CHAR (15)     NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL
    CONSTRAINT [PK_SubmissionSource] PRIMARY KEY CLUSTERED ([SubmissionSourceID] ASC),
    CONSTRAINT [FK_SubmissionSource_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
)

GO
CREATE NONCLUSTERED INDEX [IX_SubmissionSource_LastModifiedUserID]
    ON [Lookup].[WorkflowType]([LastModifiedUserID] ASC);

	
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Submission Source identifier', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'SubmissionSource', @level2type = N'COLUMN', @level2name = N'SubmissionSourceID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Entity instance code', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'SubmissionSource', @level2type = N'COLUMN', @level2name = N'Code';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'SubmissionSource', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'SubmissionSource', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'SubmissionSource', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';

