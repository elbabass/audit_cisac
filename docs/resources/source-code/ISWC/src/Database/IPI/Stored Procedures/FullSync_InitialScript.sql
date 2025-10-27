CREATE PROCEDURE [IPI].[FullSync_InitialScript]
AS  
BEGIN

	/* 1. Drop Constraints */

	IF EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Creator_Name_IPNameNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Creator] DROP CONSTRAINT [FK_Creator_Name_IPNameNumber];
	END

	IF EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Creator_InterestedParty_IPBaseNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Creator] DROP CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber];
	END

	IF EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Publisher_Name_IPNameNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Publisher] DROP CONSTRAINT [FK_Publisher_Name_IPNameNumber];
	END

	IF EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Publisher_InterestedParty_IPBaseNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Publisher] DROP CONSTRAINT [FK_Publisher_InterestedParty_IPBaseNumber];
	END

	/* 2. Truncate tables */

	TRUNCATE TABLE [IPI].[NameReference]
	TRUNCATE TABLE [IPI].[IPNameUsage]
	TRUNCATE TABLE [IPI].[Status]
	TRUNCATE TABLE [IPI].[Agreement]
	DELETE FROM [IPI].[Name]
	DELETE FROM [IPI].[InterestedParty]

END