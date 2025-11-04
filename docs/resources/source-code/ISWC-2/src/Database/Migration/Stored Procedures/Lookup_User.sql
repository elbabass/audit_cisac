
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Lookup_User]
(
   @User Migration.Azure_Lookup_User READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	INSERT INTO Lookup.[User]
	SELECT 
		u.CreatedDate, u.LastModifiedDate, 1, u.Name, u.Description
	FROM @User u
END