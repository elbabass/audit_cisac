
-- =============================================
-- Author:      Curnan Reidy
-- Create Date: 30/11/2021
-- Description: DCI Assessment
-- =============================================
CREATE   PROCEDURE [Azure].[DciAssessment]
AS
BEGIN

	CREATE TABLE #titles
	(
		IswcID bigint,
		Title NVARCHAR(512),
		IswcEligible bit,
		primary key (IswcID, IswcEligible)
	);

	;with titles as (
		select 
			w.IswcID,
			row_number() over(partition by w.IswcID, IswcEligible order by w.LastModifiedDate desc, t.LastModifiedDate asc) as roworder,
			REPLACE(t.Title, char(9), ' ') as Title,
			IswcEligible
		from ISWC.WorkInfo w
			join ISWC.Title t on w.WorkInfoID = t.WorkInfoID
			join Lookup.TitleType tt on t.TitleTypeID = tt.TitleTypeID
		where 
			tt.Code = 'OT'
			and t.Status = 1 and w.Status = 1
	)
	insert into #titles
	select 
	IswcID, Title, IswcEligible from titles
	where roworder = 1

	SELECT 
		CONVERT(VARCHAR(11), [w].[SourceDatabase]) as SourceDatabase,
		LTRIM(RTRIM([w].[AgencyID])) as AgencyID,
		[w].[AgencyWorkCode],
		CASE
			WHEN [w].[IswcEligible] = CAST(1 AS bit) THEN N'0'
			ELSE N'1'
		END as LinkedWorkIdentifier,
		[i].[Iswc],
		OriginalTitle = ISNULL(
		(
			select top 1 t.Title from #titles t 
			where i.IswcID = t.IswcID and t.IswcEligible = 1 
		),(
			select top 1 t.Title from #titles t 
			where i.IswcID = t.IswcID and t.IswcEligible = 0
		))
	FROM [ISWC].[WorkInfo] AS [w]
	INNER JOIN [ISWC].[ISWC] AS [i] ON [w].[IswcID] = [i].[IswcID]
	WHERE [w].Status <> 0 AND [i].Status <> 0 AND [w].IsReplaced <> 1
	ORDER BY i.IswcID

END