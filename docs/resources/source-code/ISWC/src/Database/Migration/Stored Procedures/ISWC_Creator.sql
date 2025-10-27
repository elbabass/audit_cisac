



-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[ISWC_Creator]
(
   @Creator Migration.Azure_ISWC_Creator READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO ISWC.Creator
	([IPBaseNumber],
	[WorkInfoID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[IsDispute],
	[RoleTypeID],
	[IswcID]
	)
	SELECT
		c.IPBaseNumber, c.WorkInfoID, 1, c.CreatedDate, c.LastModifiedDate, u.UserID, 
		CASE WHEN c.IsDispute = 'N' THEN 0 ELSE 1 END AS IsDisputeBit,
		rt.RoleTypeID, c.IswcID
	FROM @Creator c
	JOIN Lookup.[User] u on c.LastModifiedUser = u.Name
	JOIN Lookup.RoleType rt on c.RoleTypeCode = rt.Code

END