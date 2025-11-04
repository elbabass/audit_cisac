
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================


CREATE PROCEDURE [Migration].[ISWC_Title]
(
   @Title Migration.Azure_ISWC_Title READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	SET IDENTITY_INSERT ISWC.Title ON

	INSERT INTO ISWC.Title
	([TitleID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[IswcID],
	[WorkInfoID],
	[StandardizedTitle],
	[Title],
	[TitleTypeID]
	)
	SELECT 
		t.TitleID, 1, t.CreatedDate, t.LastModifiedDate, u.UserID, t.IswcID, t.WorkInfoID,
		t.StandardizedTitle, t.Title, tt.TitleTypeID
	FROM @Title t
	JOIN Lookup.TitleType tt on t.TitleTypeCode = tt.Code
	JOIN Lookup.[User] u on t.LastModifiedUser = u.Name

	SET IDENTITY_INSERT ISWC.Title OFF

END