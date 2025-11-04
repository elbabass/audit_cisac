
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[IPI_Agreement]
(
   @Agreement Migration.Azure_IPI_Agreement READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	SET IDENTITY_INSERT IPI.Agreement ON

	INSERT INTO IPI.Agreement
	([AgreementID],
	[AmendedDateTime],
	[CreationClass],
	[FromDate],
	[ToDate],
	[EconomicRights],
	[Role],
	[SharePercentage],
	[SignedDate],
	[AgencyID],
	[IPBaseNumber]
	)
	SELECT
		a.AgreementID, a.AmendedDateTime, a.CreationClass, a.FromDate, a.ToDate, a.EconomicRights, a.[Role],
		a.SharePercentage, a.SignedDate, a.AgencyID, a.IPBaseNumber
	FROM @Agreement a

	SET IDENTITY_INSERT IPI.Agreement OFF
END