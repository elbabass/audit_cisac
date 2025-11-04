
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[IPI_Status]
(
   @Status Migration.Azure_IPI_Status READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO IPI.[Status]
	([AmendedDateTime],
	[FromDate],
	[ToDate],
	[ForwardingBaseNumber],
	[StatusCode],
	[IPBaseNumber]
	)
	SELECT
		s.AmendedDateTime, s.FromDate, s.ToDate, s.ForwardingBaseNumber, s.StatusCode, s.IPBaseNumber
	FROM @Status s

END