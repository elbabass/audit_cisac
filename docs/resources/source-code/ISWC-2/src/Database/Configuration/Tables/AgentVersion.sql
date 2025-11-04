CREATE TABLE [Configuration].[AgentVersion]
(
	[AgentVersionID] INT IDENTITY (1, 1) NOT NULL, 
    [Version] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_AgentVersion] PRIMARY KEY ([AgentVersionID]),

)
