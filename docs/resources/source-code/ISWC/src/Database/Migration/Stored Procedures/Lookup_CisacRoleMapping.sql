
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Lookup_CisacRoleMapping]
(
   @CisacRoleMapping Migration.Azure_Lookup_CisacRoleMapping READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	INSERT INTO Lookup.CisacRoleMapping
	([CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[RoleTypeID],
	[CisacRoleType]
	)
	SELECT
		crm.CreatedDate, crm.LastModifiedDate, u.UserID, rt.RoleTypeID, crm.CisacRoleType
	FROM @CisacRoleMapping crm
	JOIN Lookup.RoleType rt on crm.RoleType = rt.Code
	JOIN Lookup.[User] u on crm.LastModifiedUser = u.Name
END