



CREATE PROCEDURE [Migration].[Configuration_SubmissionSource]
(
   @SubmissionSource Migration.Azure_Configuration_SubmissionSource READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO Configuration.SubmissionSource
	([Code],
	[CreatedDate],
	[LastModifiedDate],
	[LastModifiedUserID]
	)
	SELECT
		s.Code, s.CreatedDate, s.LastModifiedDate, u.UserID
	FROM @SubmissionSource s
	JOIN Lookup.[User] u on s.LastModifiedUser = u.Name

END