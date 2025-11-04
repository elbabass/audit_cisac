
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[ISWC_Performer]
(
   @Performer Migration.Azure_ISWC_Performer READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	SET IDENTITY_INSERT ISWC.Performer ON

	INSERT INTO ISWC.Performer
	([PerformerID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[FirstName],
	[LastName]
	)
	SELECT
		p.PerformerID, 1, p.CreatedDate, p.LastModifiedDate, u.UserID, p.FirstName, p.LastName
	FROM @Performer p
	JOIN Lookup.[User] u on p.LastModifiedUser = u.Name

	SET IDENTITY_INSERT ISWC.Performer OFF
END