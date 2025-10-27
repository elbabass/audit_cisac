CREATE TABLE [IPI].[InterestedParty] (
    [IPBaseNumber]    NVARCHAR (13) NOT NULL,
    [AmendedDateTime] DATETIME2 (0) NOT NULL,
    [BirthDate]       DATETIME2 (0) NULL,
    [BirthPlace]      NVARCHAR (30) NULL,
    [BirthState]      NVARCHAR (30) NULL,
    [DeathDate]       DATETIME2 (0) NULL,
    [Gender]          CHAR (1)      NULL,
    [Type]            CHAR (1)      NOT NULL,
    [AgencyID]        CHAR (3)      NOT NULL,
    [Concurrency]     ROWVERSION    NOT NULL,
    CONSTRAINT [PK_InterestedParty] PRIMARY KEY CLUSTERED ([IPBaseNumber] ASC),
    CONSTRAINT [FK_InterestedParty_Agency_AgencyID] FOREIGN KEY ([AgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID])
);




GO
CREATE NONCLUSTERED INDEX [IX_InterestedParty_AgencyID]
    ON [IPI].[InterestedParty]([AgencyID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party base number', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'IPBaseNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Amended date/time', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'AmendedDateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Birth date', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'BirthDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Birth place', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'BirthPlace';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Birth state', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'BirthState';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Death date', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'DeathDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Gender', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'Gender';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party type', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party agency', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'InterestedParty', @level2type = N'COLUMN', @level2name = N'AgencyID';

