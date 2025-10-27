CREATE TABLE [Lookup].[Language] (
    [LanguageTypeID] INT            IDENTITY (1, 1) NOT NULL,
    [LanguageCode]   NVARCHAR (5)   NOT NULL,
    [Description]    NVARCHAR (200) NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([LanguageTypeID] ASC)
);

