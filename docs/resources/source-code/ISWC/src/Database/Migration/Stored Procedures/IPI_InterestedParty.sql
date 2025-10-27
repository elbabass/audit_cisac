
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[IPI_InterestedParty]
(
   @InterestedParty Migration.Azure_IPI_InterestedParty READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO IPI.InterestedParty
	([IPBaseNumber],
	[AmendedDateTime],
	[BirthDate],
	[BirthPlace],
	[BirthState],
	[DeathDate],
	[Gender],
	[Type],
	[AgencyID]
	)
	SELECT
		i.IPBaseNumber, i.AmendedDateTime, i.BirthDate, i.BirthPlace, i.BirthState, i.DeathDate, i.Gender,
		i.[Type], a.AgencyID 
	FROM @InterestedParty i
	JOIN [Lookup].Agency a on i.AgencyID = a.AgencyID 

END