CREATE TABLE [IPI].[Agreement] (
    [AgreementID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [AmendedDateTime] DATETIME2 (0)  NOT NULL,
    [CreationClass]   CHAR (2)       NOT NULL,
    [FromDate]        DATETIME2 (0)  NOT NULL,
    [ToDate]          DATETIME2 (0)  NOT NULL,
    [EconomicRights]  CHAR (2)       NOT NULL,
    [Role]            CHAR (2)       NOT NULL,
    [SharePercentage] DECIMAL (5, 2) NOT NULL,
    [SignedDate]      DATETIME2 (0)  NULL,
    [AgencyID]        CHAR (3)       NOT NULL,
    [IPBaseNumber]    NVARCHAR (13)  NOT NULL,
    CONSTRAINT [PK_Agreement] PRIMARY KEY CLUSTERED ([AgreementID] ASC),
    CONSTRAINT [FK_Agreement_Agency_AgencyID] FOREIGN KEY ([AgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID]),
    CONSTRAINT [FK_Agreement_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber])
);








GO
CREATE NONCLUSTERED INDEX [IX_Agreement_AgencyID]
    ON [IPI].[Agreement]([AgencyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Agreement_IPBaseNumber]
    ON [IPI].[Agreement]([IPBaseNumber] ASC)
    INCLUDE([AmendedDateTime], [CreationClass], [FromDate], [ToDate], [EconomicRights], [Role], [SharePercentage], [SignedDate], [AgencyID]);






GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Membership agreements identifier', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'AgreementID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Amended date/time', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'AmendedDateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Creation class', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'CreationClass';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time of commencement', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'FromDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time of termination', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'ToDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Economic rights', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'EconomicRights';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Role within the creation class', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'Role';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Share percentage', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'SharePercentage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Agreement date/time of sign off', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'SignedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Membership agency', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'AgencyID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party base number reference', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Agreement', @level2type = N'COLUMN', @level2name = N'IPBaseNumber';


GO
CREATE NONCLUSTERED INDEX [IX_Agreement_CreationClass_FromDate_ToDate_IPBaseNumber]
    ON [IPI].[Agreement]([CreationClass] ASC, [FromDate] ASC, [ToDate] ASC, [IPBaseNumber] ASC)
    INCLUDE([EconomicRights], [AgencyID]);


GO
CREATE NONCLUSTERED INDEX [IX_Agreement_IPBaseNumber_FromDate_ToDate_EconomicRights_AgencyID_Includes]
    ON [IPI].[Agreement]([IPBaseNumber] ASC, [FromDate] ASC, [ToDate] ASC, [EconomicRights] ASC, [AgencyID] ASC)
    INCLUDE([AmendedDateTime], [CreationClass], [Role], [SharePercentage], [SignedDate]) WITH (FILLFACTOR = 100);


GO
CREATE NONCLUSTERED INDEX [IX_Agreement_CreationClass_IPBaseNumber_FromDate_ToDate]
    ON [IPI].[Agreement]([CreationClass] ASC, [IPBaseNumber] ASC, [FromDate] ASC, [ToDate] ASC)
    INCLUDE([EconomicRights], [AgencyID]) WITH (FILLFACTOR = 100);


GO
CREATE NONCLUSTERED INDEX [IX_Agreement_AgencyID_IPBaseNumber_FromDate_ToDate_EconomicRights_Includes]
    ON [IPI].[Agreement]([AgencyID] ASC, [IPBaseNumber] ASC, [FromDate] ASC, [ToDate] ASC, [EconomicRights] ASC)
    INCLUDE([AmendedDateTime], [CreationClass], [Role], [SharePercentage], [SignedDate]) WITH (FILLFACTOR = 100);


GO
CREATE NONCLUSTERED INDEX [IX_Agreement_AgencyID_IPBaseNumber]
    ON [IPI].[Agreement]([AgencyID] ASC, [IPBaseNumber] ASC) WITH (FILLFACTOR = 100);

