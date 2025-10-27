
-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[ISWC_WorkInfo]
(
   @WorkInfo Migration.Azure_ISWC_WorkInfo READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	SET IDENTITY_INSERT ISWC.WorkInfo ON
	INSERT INTO ISWC.WorkInfo
	([WorkInfoID],
	[Status],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID],
	[IswcID],
	[ArchivedIswc],
	[CisnetLastModifiedDate],
	[CisnetCreatedDate],
	[IPCount],
	[IsReplaced],
	[IswcEligible],
	[MatchTypeID],
	[MwiCategory],
	[AgencyID],
	[AgencyWorkCode],
	[SourceDatabase]
	)
	SELECT
		wi.WorkInfoID, 1, wi.CreatedDate, wi.LastModifiedDate, u.UserID, wi.IswcID,
		wi.ArchivedIswc, wi.CisnetLastModifiedDate, wi.CisnetCreatedDate, wi.IPCount,
		CASE WHEN wi.IsReplaced = 'N' THEN 0 ELSE 1 END AS IsReplacedBit,
		CASE WHEN wi.IswcEligible = 'N' THEN 0 ELSE 1 END AS IswcEligibleBit,
		mt.MatchTypeID, wi.MwiCategory, wi.AgencyID, wi.AgencyWorkCode, wi.SourceDatabase
	FROM @WorkInfo wi
	JOIN Lookup.[User] u on wi.LastModifiedUser = u.Name
	JOIN Lookup.MatchType mt on wi.MatchTypeCode = mt.Code

	SET IDENTITY_INSERT ISWC.WorkInfo OFF
	
END