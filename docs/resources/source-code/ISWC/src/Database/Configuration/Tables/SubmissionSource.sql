CREATE TABLE [Configuration].[SubmissionSource] (
    [SubmissionSourceID] INT           IDENTITY (1, 1) NOT NULL,
    [Code]               CHAR (15)     NOT NULL,
    [CreatedDate]        DATETIME2 (0) NOT NULL,
    [LastModifiedDate]   DATETIME2 (0) NOT NULL,
    [LastModifiedUserID] INT           NOT NULL,
    CONSTRAINT [PK_SubmissionSource] PRIMARY KEY CLUSTERED ([SubmissionSourceID] ASC),
    CONSTRAINT [FK_SubmissionSource_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);

