CREATE TABLE [Lookup].[IswcStatus] (
    [IswcStatusID] INT           IDENTITY (1, 1) NOT NULL,
    [Code]             NVARCHAR (20) NOT NULL,
    [Description]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_IswcStatusID] PRIMARY KEY CLUSTERED ([IswcStatusID] ASC)
);