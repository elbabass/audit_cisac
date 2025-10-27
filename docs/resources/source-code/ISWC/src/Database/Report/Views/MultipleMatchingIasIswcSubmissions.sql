CREATE VIEW [Report].[MultipleMatchingIasIswcSubmissions]
AS
with					
    Works			
    as			
    (			
        Select distinct			
			Iswc,
            Title,			
            AgencyWorkCode,			
            string_agg(cast(c.IPBaseNumber as NVARCHAR(MAX)), ',') within group (order by c.IPBaseNumber) Creators,			
			wi.CreatedDate WorkCreationDate
        from ISWC.WorkInfo wi			
			inner join ISWC.ISWC i on wi.IswcID = i.IswcID
            inner join ISWC.Title t on wi.WorkInfoID = t.WorkInfoID and t.TitleTypeID = 2				
			inner join ISWC.Creator c on wi.WorkInfoID = c.WorkInfoID
			
        where AgencyWorkCode like 'AS%'			
		and wi.Status = 1 	
		and wi.IsReplaced <> 1	
		and i.Status = 1	
		group by Iswc, Title, AgencyWorkCode, wi.CreatedDate
    ),			
	Matches		
	as		
	(		
		select 	
			string_agg(cast(Iswc as NVARCHAR(MAX)), ',') within group (order by WorkCreationDate) ISWCs,
			Title,
			string_agg(cast(AgencyWorkCode as NVARCHAR(MAX)), ',') as AgencyWorkCodes,
			Creators,
			string_agg(cast(concat(AgencyWorkCode, ' : ', WorkCreationDate) as NVARCHAR(MAX)), ', ') as WorkCreationDates,
			Count(*) as MultipleISWCsCount
		from Works	
		group by 	
		Title,	
		Creators	
		having count(*) > 1	
	)		
Select *			
from (			
	Select *,		
        (Select count(distinct value)			
        from string_split(Matches.ISWCs, ',')) 			
			 as DistinctISWCIDs
    from Matches			
) as FilterSelect			
where DistinctISWCIDs > 1