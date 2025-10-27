
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================


CREATE PROCEDURE [Migration].[ISWC_Publisher]
(
   @Publisher Migration.Azure_ISWC_Publisher READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	SET IDENTITY_INSERT ISWC.Publisher ON

	INSERT INTO ISWC.Publisher
	([IPBaseNumber],
	[WorkInfoID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[RoleTypeID],
	[IswcID],
	[IPNameNumber]
	)
	SELECT
		p.IPBaseNumber, p.WorkInfoID, 1, p.CreatedDate, p.LastModifiedDate, u.UserID, p.RoleTypeCode, p.IswcID, p.IPNameNumber
	FROM @Publisher p
	JOIN Lookup.[User] u on p.LastModifiedUser = u.Name

	SET IDENTITY_INSERT ISWC.Publisher OFF
END