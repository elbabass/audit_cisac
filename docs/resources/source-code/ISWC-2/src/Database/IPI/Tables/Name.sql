CREATE TABLE [IPI].[Name] (
    [IPNameNumber]         BIGINT        NOT NULL,
    [AmendedDateTime]      DATETIME2 (0) NOT NULL,
    [FirstName]            NVARCHAR (45) NULL,
    [LastName]             NVARCHAR (90) NOT NULL,
    [CreatedDate]          DATETIME2 (0) NOT NULL,
    [TypeCode]             CHAR (2)      NOT NULL,
    [ForwardingNameNumber] BIGINT        NULL,
    [AgencyID]             CHAR (3)      NULL,
    CONSTRAINT [PK_Name] PRIMARY KEY CLUSTERED ([IPNameNumber] ASC),
    CONSTRAINT [FK_Name_Agency_AgencyID] FOREIGN KEY ([AgencyID]) REFERENCES [Lookup].[Agency] ([AgencyID])
);




GO
CREATE NONCLUSTERED INDEX [IX_Name_AgencyID]
    ON [IPI].[Name]([AgencyID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party name number', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'IPNameNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Amended date/time', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'AmendedDateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'First Name', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'FirstName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Last Name', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'LastName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Date/time the name entry was created', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'CreatedDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Type of name entry', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'TypeCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Forwarding name number', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'ForwardingNameNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Name entry agency', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'Name', @level2type = N'COLUMN', @level2name = N'AgencyID';


GO
CREATE NONCLUSTERED INDEX [IX_Name_IPNameNumber]
    ON [IPI].[Name]([IPNameNumber] ASC)
    INCLUDE([AmendedDateTime], [CreatedDate], [FirstName], [ForwardingNameNumber], [LastName], [TypeCode], [AgencyID]);

