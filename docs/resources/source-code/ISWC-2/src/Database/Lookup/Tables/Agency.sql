CREATE TABLE [Lookup].[Agency] (
    [AgencyID]                      CHAR (3)      NOT NULL,
    [Name]                          NVARCHAR (50) NOT NULL,
    [Country]                       NVARCHAR (50) NULL,
    [CreatedDate]                   DATETIME2 (0) NOT NULL,
    [LastModifiedDate]              DATETIME2 (0) NOT NULL,
    [LastModifiedUserID]            INT           NOT NULL,
    [ISWCStatus]                    INT           NOT NULL,
    [DisallowDisambiguateOverwrite] NVARCHAR (50) DEFAULT ('REST;FILE;CISNET') NOT NULL,
    [EnableChecksumValidation]      BIT           CONSTRAINT [DF_Agency_EnableChecksumValidation] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Agency] PRIMARY KEY CLUSTERED ([AgencyID] ASC),
    CONSTRAINT [FK_Agency_User_LastModifiedUserID] FOREIGN KEY ([LastModifiedUserID]) REFERENCES [Lookup].[User] ([UserID])
);




GO
CREATE NONCLUSTERED INDEX [IX_Agency_LastModifiedUserID]
    ON [Lookup].[Agency]([LastModifiedUserID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Agency identifier', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'AgencyID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Agency name', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Country', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'Country';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date that the entity instance was created', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date of last modification', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'LastModifiedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'The last modifying user', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'LastModifiedUserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '0 for NoISWCAccess, 1 for ISWCAccess', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'ISWCStatus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Disallow overwrite of disambiguation data from these sources', @level0type = N'SCHEMA', @level0name = N'Lookup', @level1type = N'TABLE', @level1name = N'Agency', @level2type = N'COLUMN', @level2name = N'DisallowDisambiguateOverwrite';

