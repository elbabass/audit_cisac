
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Lookup_MatchType]
(
   @MatchType Migration.Azure_Lookup_MatchType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	INSERT INTO Lookup.MatchType
	([Code],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[Description])
	SELECT 
		mt.Code, mt.CreatedDate, mt.LastModifiedDate, u.UserID, mt.Description
	FROM @MatchType mt
	JOIN Lookup.[User] u on mt.LastModifiedUser = u.Name
END