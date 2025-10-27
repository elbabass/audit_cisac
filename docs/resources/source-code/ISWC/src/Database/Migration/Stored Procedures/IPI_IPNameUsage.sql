
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[IPI_IPNameUsage]
(
   @IPNameUsage Migration.Azure_IPI_IPNameUsage READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO IPI.IPNameUsage
	([IPBaseNumber],
	[Role],
	[CreationClass],
	[IPNameNumber]
	)
	SELECT
		nu.IPBaseNumber, nu.[Role], nu.CreationClass, nu.IPNameNumber
	FROM @IPNameUsage nu

END