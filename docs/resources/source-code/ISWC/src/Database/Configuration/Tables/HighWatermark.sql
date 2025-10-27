CREATE TABLE [Configuration].[HighWatermark]
(
	[Name] NVARCHAR(100) NOT NULL,
	[Value] DATETIME NULL, 
    CONSTRAINT [PK_HWM_Name] PRIMARY KEY ([Name])
)
