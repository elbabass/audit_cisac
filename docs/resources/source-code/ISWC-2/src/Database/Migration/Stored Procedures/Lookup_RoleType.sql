
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Lookup_RoleType]
(
   @RoleType Migration.Azure_Lookup_RoleType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	INSERT INTO Lookup.RoleType
	([Code],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[Description]
	)
	SELECT 
		rt.Code, rt.CreatedDate, rt.LastModifiedDate, u.UserID, rt.Description
	FROM @RoleType rt
	JOIN Lookup.[User] u on rt.LastModifiedUser = u.Name
END