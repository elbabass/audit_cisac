
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[ISWC_WorkInfoPerformer]
(
   @WorkInfoPerformer Migration.Azure_ISWC_WorkInfoPerformer READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO ISWC.WorkInfoPerformer
	([WorkInfoID],
	[PerformerID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID]
	)
	SELECT
		wip.WorkInfoID, wip.PerformerID, 1, wip.CreatedDate, wip.LastModifiedDate, u.UserID
	FROM @WorkInfoPerformer wip
	JOIN Lookup.[User] u on wip.LastModifiedUser = u.Name

END