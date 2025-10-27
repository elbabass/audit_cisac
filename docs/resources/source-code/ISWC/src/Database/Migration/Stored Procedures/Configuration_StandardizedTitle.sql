


-- =============================================
-- Author: Dylan Mac Namee
-- Create Date:20/08/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Configuration_StandardizedTitle]
(
   @StandardizedTitle Migration.Azure_Configuration_StandardizedTitle READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [Configuration].[StandardizedTitle]
	([Society],
	[SearchPattern],
	[ReplacePattern]
	)
	SELECT
		s.[Society], s.[SearchPattern], s.[ReplacePattern]
	FROM @StandardizedTitle s

END