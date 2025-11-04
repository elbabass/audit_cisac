
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[Lookup_TitleTypes]
(
   @TitleTypes Migration.Azure_Lookup_TitleTypes READONLY
)
AS
BEGIN

	SET NOCOUNT ON

	INSERT INTO Lookup.TitleType
	SELECT 
		Code, tt.CreatedDate, tt.LastModifiedDate, u.UserID, tt.Description
	FROM @TitleTypes tt
	JOIN Lookup.[User] u on tt.LastModifiedUser = u.Name
   
END