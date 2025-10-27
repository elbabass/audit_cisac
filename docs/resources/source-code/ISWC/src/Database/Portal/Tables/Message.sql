CREATE TABLE [Portal].[Message](
	[MessageID] INT IDENTITY(1,1) NOT NULL,
	[Header] NVARCHAR(250) NOT NULL,
	[MessageBody] NVARCHAR(1500) NOT NULL,
	[Status] BIT NOT NULL,
	[CreatedDate] DATETIME2(0) NOT NULL,
	[LastModifiedDate] DATETIME2(0) NOT NULL,
	[Portal] NVARCHAR(10) NOT NULL,
	[LanguageTypeID] INT NOT NULL
	CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED([MessageID] ASC),
    CONSTRAINT [FK_Message_LanguageType] FOREIGN KEY ([LanguageTypeID]) REFERENCES [Lookup].[Language] ([LanguageTypeID])
)
GO