CREATE VIEW [Azure].[Index_Repertoire_Work]
AS
WITH Dataset AS
(
	select d.WorkInfoID from 
	(
		select WorkInfoID, LastModifiedDate from [ISWC].[WorkInfo]
		union all
		select WorkInfoID, LastModifiedDate from [ISWC].[Title]
		union all
		select WorkInfoID, LastModifiedDate from [ISWC].[Creator]
	) d
	WHERE LastModifiedDate > DATEADD(HOUR, -15, getdate())
	group by WorkInfoID
)
SELECT 
	wi.WorkInfoID AS GeneratedID,
	wi.WorkInfoID AS WorkID,
	NULL as [MedleyType],
	NULL AS SocietyAccountNumber,
	~wi.Status AS IsDeleted,
	wi.IswcEligible AS IsEligible,
	CONVERT(bit, 1) AS ExistsInDatabase,
	wi.Concurrency,
	(
		select
			i.IswcStatusID
		from 
			ISWC.ISWC i
		where
			wi.IswcID = i.IswcID	
	) as IswcStatusID,
	(
		select
			t.Title AS WorkName,
			t.TitleTypeID AS WorkNameType
		from 
			ISWC.Title t 
		where t.WorkInfoID = wi.WorkInfoID 
			and t.Status = 1
			and wi.IswcEligible = 1
			and t.IswcID = wi.IswcID
		FOR JSON PATH
	) as WorkNames,
	(
		select * from(
		select 
			case
				when (w.ArchivedIswc is not null and w.ArchivedIswc <> i.Iswc and numbers.Type = 'Archived') then 'ARCHIVED'
				when numbers.Type = 'Iswc' then 'ISWC'
				when numbers.Type = 'AgencyWorkCode' then w.AgencyID
				else null
			end as TypeCode,
			numbers.Number
		from [ISWC].[WorkInfo] w
			join ISWC.ISWC i on w.IswcID = i.IswcID
			cross apply ( values ('AgencyWorkCode', AgencyWorkCode), ('Iswc', Iswc), ('Archived', ArchivedIswc)) numbers (Type, Number)
		where w.WorkInfoID = wi.WorkInfoID
			and w.IswcID = wi.IswcID
		) as arc
		where TypeCode is not null
		FOR JSON PATH
	) as WorkNumbers,
	(
		select
			case
				when (ip.IPNameNumber is not null) then ip.IPNameNumber
				else 0
			end as PersonID,
			ip.IPBaseNumber AS IPIBaseNumber,
			ip.CreatedDate AS IPICreatedDate,
			case
				when (ip.IPBaseNumber is not null) then trim(concat(n.LastName, ' ', n.FirstName))
				else trim(concat(ip.LastName, ' ',ip.FirstName))
			end as PersonFullName,
			case
				when (ip.IPBaseNumber is not null) then n.LastName
				else ip.LastName
			end as LastName,
			2 AS PersonType,
			2 AS ContributorType,
			ip.IPNameNumber AS IPINumber,
			(select Top 1 Code from [Lookup].[RoleType] rt where rt.RoleTypeID = ip.RoleTypeID) as RoleType
		from [ISWC].[Creator] ip
			left join IPI.Name n on n.IPNameNumber = ip.IPNameNumber
		where ip.WorkInfoID = wi.WorkInfoID
			and ip.IswcID = wi.IswcID
			and ip.Status = 1
			and wi.IswcEligible = 1
			and wi.Status = 1
		FOR JSON PATH
	) as WorkContributors,
	(
		select
			wip.PerformerID AS PersonID,
			concat(p.LastName, ' ', p.FirstName) AS PersonFullName,
			p.LastName
		from [ISWC].[WorkInfoPerformer] wip
			inner join [ISWC].[Performer] p on wip.PerformerID = p.PerformerID
		where wip.WorkInfoID = wi.WorkInfoID
		and wi.IswcEligible = 1
		and wip.Status = 1
		FOR JSON PATH
	) as WorkPerformers
FROM 
	[ISWC].[WorkInfo] wi
	where wi.WorkInfoID in (select WorkInfoID from Dataset)
GO