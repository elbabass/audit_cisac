CREATE TABLE [Configuration].[StandardizedTitle] (
    [StandardizedTitleID] INT           IDENTITY (1, 1) NOT NULL,
    [Society]             CHAR (3)      NOT NULL,
    [SearchPattern]       VARCHAR (150) NOT NULL,
    [ReplacePattern]      VARCHAR (150) NULL,
    CONSTRAINT [PK_StandardizedTitle] PRIMARY KEY CLUSTERED ([StandardizedTitleID] ASC)
);



