
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[IPI_NameReference]
(
   @NameReference Migration.Azure_IPI_NameReference READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO IPI.NameReference
	([IPNameNumber],
	[AmendedDateTime],
	[IPBaseNumber]
	)
	SELECT
		nr.IPNameNumber, nr.AmendedDateTime, nr.IPBaseNumber
	FROM @NameReference nr

END