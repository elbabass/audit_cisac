
-- =============================================
-- Author:      Dylan Mac Namee
-- Create Date: 20/01/21
-- =============================================

CREATE PROCEDURE [dbo].[DeletePtAndCtTitles]
AS
DECLARE @workinfoIds TABLE (id bigint);
BEGIN
BEGIN TRANSACTION

	UPDATE ISWC.Title
	SET Status = 0
	OUTPUT INSERTED.WorkInfoID INTO @workinfoIds
	WHERE TitleTypeID = 1 or TitleTypeID = 13;

	UPDATE w
	SET w.LastModifiedDate = GETDATE()
	FROM ISWC.WorkInfo w
	JOIN @workinfoIds wi ON wi.id = w.WorkInfoID

COMMIT TRANSACTION
END