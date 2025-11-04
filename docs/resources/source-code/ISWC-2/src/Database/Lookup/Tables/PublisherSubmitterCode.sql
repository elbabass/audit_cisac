CREATE TABLE [Lookup].[PublisherSubmitterCode](
	[PublisherSubmitterCodeID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Publisher] [nvarchar](100) NOT NULL,
	[IPNameNumber] [bigint] NULL,
 CONSTRAINT [PK_PublisherSubmitterCode] PRIMARY KEY CLUSTERED ([PublisherSubmitterCodeID] ASC)
);
