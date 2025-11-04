CREATE TABLE [Lookup].[Instrumentation] (
    [InstrumentationID]  INT            IDENTITY (1, 1) NOT NULL,
    [CreatedDate]        DATETIME2 (0)  NOT NULL,
    [LastModifiedDate]   DATETIME2 (0)  NOT NULL,
    [LastModifiedUserID] INT            NOT NULL,
    [Name]               NVARCHAR (50)  NOT NULL,
    [Code]               CHAR (3)       NOT NULL,
    [Family]             NVARCHAR (50)   NOT NULL,
    [Note]               NVARCHAR (450) NULL,
    CONSTRAINT [PK_Instrumentation] PRIMARY KEY CLUSTERED ([InstrumentationID] ASC),
    CONSTRAINT [FK_Instrumentation_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Instrumentation_LastModifiedUserID]
    ON [Lookup].[Instrumentation]([LastModifiedUserID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Instrumentation identifier', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'InstrumentationID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Instrument name', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Instrumentation type code', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'Code';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Instrument family', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'Family';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Additional notes for instrument', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Instrumentation', @level2type = N'COLUMN', @level2name = N'Note';

