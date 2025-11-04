CREATE VIEW [Report].[DuplicatePublisherIasSubmissions]
AS
with				
    Works				
    as				
    (				
        Select distinct				
			i.IswcID,	
            Title,				
            AgencyWorkCode,				
            string_agg(c.IPBaseNumber, ',') within group (order by c.IPBaseNumber) Creators,				
			string_agg(c.IPNameNumber, ',') NameNumbers,	
			WorkIdentifier as PublisherWorkCode,	
			(select Code from Lookup.NumberType 	
				where NumberTypeID = ai.NumberTypeID) Publisher
        from ISWC.WorkInfo wi				
			inner join ISWC.ISWC i on wi.IswcID = i.IswcID	
            inner join ISWC.Title t on wi.WorkInfoID = t.WorkInfoID and t.TitleTypeID = 2				
			inner join ISWC.Creator c on wi.WorkInfoID = c.WorkInfoID	
			inner join ISWC.AdditionalIdentifier ai 	
			on ai.AdditionalIdentifierID = (select top 1 AdditionalIdentifierID from ISWC.AdditionalIdentifier where WorkInfoID = wi.WorkInfoID and NumberTypeID <> 1 order by 1 desc)	
				
        where AgencyWorkCode like 'AS%'				
		and wi.Status = 1 		
		and wi.IsReplaced <> 1		
		group by i.IswcID, Title, AgencyWorkCode, WorkIdentifier, NumberTypeID		
    ),				
	Comparison			
	as			
	(			
		select l.*, 		
			r.IswcID IswcId2, r.Title Title2, r.AgencyWorkCode AgencyWorkCode2, r.Creators Creators2, r.NameNumbers NameNumbers2, r.PublisherWorkCode PublisherWorkCode2, r.Publisher Publisher2	
		from Works l		
		inner join Works r on l.PublisherWorkCode = r.PublisherWorkCode and l.Publisher = r.Publisher		
		and l.IswcID <> r.IswcID and l.AgencyWorkCode <> r.AgencyWorkCode and l.Creators <> r.Creators		
	)			
	select 			
		(select Iswc from ISWC.ISWC where IswcID = c.IswcID) Iswc, 		
		c.Title Title,		
		c.Creators Creators,		
		c.NameNumbers NameNumbers,		
		c.PublisherWorkCode PublisherWorkCode,		
		c.Publisher Publisher,		
		(select Iswc from ISWC.ISWC where IswcID = c.IswcID2) Iswc2, 		
		c.Title2 Title2,		
		c.Creators2 Creators2,		
		c.NameNumbers2 NameNumbers2,		
		c.PublisherWorkCode2 PublisherWorkCode2,		
		c.Publisher2 Publisher2		
	from Comparison c			
	group by IswcID, c.Title, c.Creators, c.NameNumbers , c.PublisherWorkCode, c.Publisher, c.IswcID2, c.Title2, c.Creators2, c.NameNumbers2, c.PublisherWorkCode2, c.Publisher2