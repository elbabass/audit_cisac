

-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Lookup_Agency]
(
   @Agency Migration.Azure_Lookup_Agency READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO Lookup.Agency
	([AgencyID],
	[Name],
	[Country],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[ISWCStatus]
	)
	SELECT 
		a.AgencyID, a.Name, a.Country, a.CreatedDate, a.LastModifiedDate, u.UserID,
		CASE
			WHEN a.AgencyID IN (251, 253, 256, 257, 258, 259, 260, 261, 268, 270, 271, 272, 276, 279, 283, 294, 319)
			THEN 0 ELSE 1
		END AS ISWCStatusBit
		FROM @Agency a
	JOIN Lookup.[User] u on a.LastModifiedUser = u.Name

END