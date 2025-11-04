CREATE TYPE [Migration].[Azure_Configuration_StandardizedTitle] AS TABLE (
    [Society]        CHAR (3)      NOT NULL,
    [SearchPattern]  VARCHAR (150) NOT NULL,
    [ReplacePattern] VARCHAR (150) NULL);

