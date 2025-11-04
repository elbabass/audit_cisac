
-- =============================================
-- Author:      Dylan Mac Namee
-- Create Date: 20/01/21
-- =============================================
CREATE PROCEDURE [dbo].[DeleteEtTitles]
(@agency_list nvarchar(MAX))
AS
DECLARE @workinfoIds TABLE (id bigint);
BEGIN
BEGIN TRANSACTION

   UPDATE t
   SET t.Status = 0
   OUTPUT INSERTED.WorkInfoID INTO @workinfoIds
   FROM ISWC.Title t
   INNER JOIN ISWC.WorkInfo w on t.WorkInfoID = w.WorkInfoID
   WHERE t.TitleTypeID = 5
   AND w.AgencyID IN (SELECT VALUE FROM STRING_SPLIT(@agency_list, ','))

   UPDATE w
   SET w.LastModifiedDate = GETDATE()
   FROM ISWC.WorkInfo w
   JOIN @workinfoIds wi ON wi.id = w.WorkInfoID

COMMIT TRANSACTION
END