CREATE VIEW [Report].[SingleIasIswcSubmissions]
AS
with allocationIswcs as (	
	select i.Iswc, n.Code, ai.WorkIdentifier, wi.CreatedDate
	from ISWC.ISWC i
	join ISWC.WorkInfo wi on i.IswcID = wi.IswcID and wi.Status = 1
	join ISWC.AdditionalIdentifier ai on ai.WorkInfoID = wi.WorkInfoID
	join Lookup.NumberType n on n.NumberTypeID = ai.NumberTypeID
	where wi.AgencyWorkCode like 'AS%'
),	
iswcs as (	
	select Iswc, count(9) as cnt
	from ISWC.ISWC i
	join ISWC.WorkInfo wi on i.IswcID = wi.IswcID and wi.Status = 1
	group by Iswc
)	
select a.Iswc, code as Publisher, workidentifier as WorkIdentifier, CreatedDate	
from allocationIswcs a	
join iswcs i on a.iswc = i.iswc	
where cnt = 1