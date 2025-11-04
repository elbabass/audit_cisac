
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[ISWC_ISWC]
(
   @ISWC Migration.Azure_ISWC_ISWC READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	SET IDENTITY_INSERT ISWC.ISWC ON

	INSERT INTO ISWC.ISWC
	([IswcID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[Iswc],
	[AgencyID]
	)
	SELECT
		i.IswcID, 1, i.CreatedDate, i.LastModifiedDate, u.UserID, i.Iswc, i.AgencyID
	FROM @ISWC i
	JOIN Lookup.[User] u on i.LastModifiedUser = u.Name

	SET IDENTITY_INSERT ISWC.ISWC OFF

END