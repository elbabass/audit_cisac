CREATE PROCEDURE [IPI].[FullSync_FinalScript]
AS  
BEGIN  

	/* 1. Add Constraints */

	IF NOT EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Creator_Name_IPNameNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Creator] WITH NOCHECK ADD CONSTRAINT [FK_Creator_Name_IPNameNumber] FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name] ([IPNameNumber]);
	  ALTER TABLE [ISWC].[Creator] CHECK CONSTRAINT [FK_Creator_Name_IPNameNumber]
	END

	IF NOT EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Creator_InterestedParty_IPBaseNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Creator] WITH NOCHECK ADD CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber]);
	  ALTER TABLE [ISWC].[Creator] CHECK CONSTRAINT [FK_Creator_InterestedParty_IPBaseNumber]
	END

	IF NOT EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Publisher_Name_IPNameNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Publisher] WITH NOCHECK ADD CONSTRAINT [FK_Publisher_Name_IPNameNumber] FOREIGN KEY ([IPNameNumber]) REFERENCES [IPI].[Name] ([IPNameNumber]);
	  ALTER TABLE [ISWC].[Publisher] CHECK CONSTRAINT [FK_Publisher_Name_IPNameNumber]
	END

	IF NOT EXISTS (SELECT
		1
	  FROM sys.foreign_keys
	  WHERE name = 'FK_Publisher_InterestedParty_IPBaseNumber')
	BEGIN
	  ALTER TABLE [ISWC].[Publisher] WITH NOCHECK ADD CONSTRAINT [FK_Publisher_InterestedParty_IPBaseNumber] FOREIGN KEY ([IPBaseNumber]) REFERENCES [IPI].[InterestedParty] ([IPBaseNumber]);
	  ALTER TABLE [ISWC].[Publisher] CHECK CONSTRAINT [FK_Publisher_InterestedParty_IPBaseNumber]
	END

END