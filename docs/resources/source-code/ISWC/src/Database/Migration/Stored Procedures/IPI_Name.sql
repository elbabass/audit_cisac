-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[IPI_Name]
(
   @Name Migration.Azure_IPI_Name READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO IPI.[Name]
	([IPNameNumber],
	[AmendedDateTime],
	[FirstName],
	[LastName],
	[CreatedDate],
	[TypeCode],
	[ForwardingNameNumber],
	[AgencyID]
	)
	SELECT
		n.IPNameNumber, n.AmendedDateTime, n.FirstName, n.LastName, n.CreatedDate, n.TypeCode, n.ForwardingNameNumber,
		a.AgencyID
	FROM @Name n
	JOIN [Lookup].Agency a on n.AgencyID = a.AgencyID 

END