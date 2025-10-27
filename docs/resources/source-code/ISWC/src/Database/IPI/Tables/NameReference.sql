CREATE TABLE [IPI].[NameReference] (
    [IPNameNumber]    BIGINT        NOT NULL,
    [AmendedDateTime] DATETIME2 (0) NOT NULL,
    [IPBaseNumber]    NVARCHAR (13) NOT NULL,
    CONSTRAINT [PK_NameReference] PRIMARY KEY CLUSTERED ([IPNameNumber] ASC, [IPBaseNumber] ASC),
    CONSTRAINT [FK_NameReference_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber]),
    CONSTRAINT [FK_NameReference_Name_IPNameNumber] FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name] ([IPNameNumber])
);






GO
CREATE NONCLUSTERED INDEX [IX_NameReference_IPBaseNumber]
    ON [IPI].[NameReference]([IPBaseNumber] ASC)
    INCLUDE([AmendedDateTime]);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party name number reference', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'NameReference', @level2type = N'COLUMN', @level2name = N'IPNameNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Amended date/time', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'NameReference', @level2type = N'COLUMN', @level2name = N'AmendedDateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Interested Party base number reference', @level0type = N'SCHEMA', @level0name = N'IPI', @level1type = N'TABLE', @level1name = N'NameReference', @level2type = N'COLUMN', @level2name = N'IPBaseNumber';


GO
CREATE NONCLUSTERED INDEX [IX_NameReference_IPNameNumber]
    ON [IPI].[NameReference]([IPNameNumber] ASC)
    INCLUDE([IPBaseNumber], [AmendedDateTime]);

