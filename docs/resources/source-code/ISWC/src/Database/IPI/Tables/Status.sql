CREATE TABLE [IPI].[Status] (
    [StatusID]             INT           IDENTITY (1, 1) NOT NULL,
    [AmendedDateTime]      DATETIME2 (0) NOT NULL,
    [FromDate]             DATETIME2 (0) NOT NULL,
    [ToDate]               DATETIME2 (0) NOT NULL,
    [ForwardingBaseNumber] NVARCHAR (13) NOT NULL,
    [StatusCode]           INT           NOT NULL,
    [IPBaseNumber]         NVARCHAR (13)  NOT NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([StatusID] ASC),
    CONSTRAINT [FK_Status_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber])
);


GO
CREATE NONCLUSTERED INDEX [IX_Status_IPBaseNumber]
    ON [IPI].[Status]([IPBaseNumber] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Status identifier', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'StatusID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Amended date/time', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'AmendedDateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time of commencement', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'FromDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time of termination', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'ToDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Forwarding base number', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'ForwardingBaseNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Status of base number', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'StatusCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party base number reference', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Status', @level2type = N'COLUMN', @level2name = N'IPBaseNumber';

