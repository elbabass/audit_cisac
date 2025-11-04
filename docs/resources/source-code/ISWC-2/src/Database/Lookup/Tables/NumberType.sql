CREATE TABLE [Lookup].[NumberType](
	[NumberTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](80) NULL,
 CONSTRAINT [PK_NumberType] PRIMARY KEY CLUSTERED ([NumberTypeID] ASC)
);