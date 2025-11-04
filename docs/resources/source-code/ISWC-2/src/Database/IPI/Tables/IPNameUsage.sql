CREATE TABLE [IPI].[IPNameUsage] (
    [IPNameNumber]  BIGINT           NOT NULL,
    [Role]          CHAR (2)      NOT NULL,
    [CreationClass] CHAR (2)      NOT NULL,
    [IPBaseNumber]  NVARCHAR (13)  NOT NULL,
    CONSTRAINT [PK_IPNameUsage] PRIMARY KEY CLUSTERED ([IPNameNumber] ASC, [Role] ASC, [CreationClass] ASC, [IPBaseNumber] ASC),
    CONSTRAINT [FK_IPNameUsage_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber]),
    CONSTRAINT [FK_IPNameUsage_Name_IPNameNumber] FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name] ([IPNameNumber])
);


GO
CREATE NONCLUSTERED INDEX [IX_IPNameUsage_IPBaseNumber]
    ON [IPI].[IPNameUsage]([IPBaseNumber] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party name number reference', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'IPNameUsage', @level2type = N'COLUMN', @level2name = N'IPNameNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Role within the creation class', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'IPNameUsage', @level2type = N'COLUMN', @level2name = N'Role';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Creation class', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'IPNameUsage', @level2type = N'COLUMN', @level2name = N'CreationClass';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party base number reference', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'IPNameUsage', @level2type = N'COLUMN', @level2name = N'IPBaseNumber';

