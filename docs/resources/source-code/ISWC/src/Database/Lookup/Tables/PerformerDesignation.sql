CREATE TABLE [Lookup].[PerformerDesignation] (
    [PerformerDesignationID] INT           IDENTITY (1, 1) NOT NULL,
    [Code]             NVARCHAR (20) NOT NULL,
    [Description]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_PerformerDesignationID] PRIMARY KEY CLUSTERED ([PerformerDesignationID] ASC)
);