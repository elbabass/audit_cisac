CREATE VIEW [Azure].[Databricks_Iswc_Data]
AS
SELECT DISTINCT
    wi.WorkInfoID AS WorkId, 
    i.Iswc AS PreferredIswc,
    lis.Code AS IswcStatus,
    wi.AgencyID AS AgencyCode,
    wi.SourceDatabase AS SourceDb,
    wi.AgencyWorkCode,
    wi.CreatedDate,
    wi.LastModifiedDate,
	wi.CisnetCreatedDate,
	wi.CisnetLastModifiedDate,
	wi.IswcEligible AS IsEligible,
    wi.Status,
    wi.IsReplaced,
	wi.Disambiguation,
	wi.DisambiguationReasonID AS DisambiguationReason,
	wi.DerivedWorkTypeID AS DerivedWorkType,
	CASE 
        WHEN wi.MwiCategory = 'DOM' THEN CAST(0 AS int)
		WHEN wi.MwiCategory = 'INT' THEN CAST(1 AS int)
        ELSE NULL
	END AS Category,
	CASE 
        WHEN wi.BVLTR = 'B' THEN CAST(0 AS int)
		WHEN wi.BVLTR = 'L' THEN CAST(1 AS int)
		WHEN wi.BVLTR = 'T' THEN CAST(2 AS int)
		WHEN wi.BVLTR = 'V' THEN CAST(3 AS int)
		WHEN wi.BVLTR = 'R' THEN CAST(4 AS int)
        ELSE NULL
	END AS BVLTR,
    JSON_QUERY(
        (SELECT 
            t.TitleID,
            t.Title AS Name,  
            t.StandardizedTitle,
            tt.Code AS Type
         FROM ISWC.Title t
         INNER JOIN Lookup.TitleType tt ON t.TitleTypeID = tt.TitleTypeID
         WHERE t.WorkInfoID = wi.WorkInfoID AND t.IswcID = wi.IswcID AND t.Status <> 0
         FOR JSON PATH
        )
    ) AS Titles,
	JSON_QUERY(
        (SELECT 
            di.Iswc
         FROM ISWC.DisambiguationISWC di
		 WHERE di.WorkInfoID = wi.WorkInfoID and di.Status <> 0
         FOR JSON PATH
        )
    ) AS DisambiguateFrom,
	JSON_QUERY(
        (SELECT 
            df.Iswc,
			df.Title
         FROM ISWC.DerivedFrom df
		 WHERE df.WorkInfoID = wi.WorkInfoID and df.Status <> 0
         FOR JSON PATH
        )
    ) AS DerivedFrom,
	JSON_QUERY(
        (SELECT 
            i.InstrumentationID Code,
			i.Name
         FROM ISWC.WorkInfoInstrumentation wii
		 INNER JOIN Lookup.Instrumentation i on wii.InstrumentationID = i.InstrumentationID 
		 WHERE wii.WorkInfoID = wi.WorkInfoID and wii.Status <> 0
         FOR JSON PATH
        )
    ) AS Instrumentation,
	JSON_QUERY(
        (SELECT 
            ai.WorkIdentifier AS WorkCode,
			ai.NumberTypeID,
			nt.Code SubmitterCode
         FROM ISWC.AdditionalIdentifier ai
		 INNER JOIN Lookup.NumberType nt on ai.NumberTypeID = nt.NumberTypeID
		 WHERE ai.WorkInfoID = wi.WorkInfoID
         FOR JSON PATH
        )
    ) AS AdditionalIdentifiers,
	JSON_QUERY(
        (SELECT DISTINCT
            ip.ContributorID,
            ip.IPBaseNumber AS IpBaseNumber, 
            ip.IPNameNumber, 
            ip.CisacType,
            ip.IsAuthoritative,
            ip.Status,
            ip.Name,
            ip.LastName,
            ip.DisplayName,
            ip.CreatedDate,
            ip.IsExcludedFromIswc,
            ip.Agency,
            ip.DeathDate,
            ip.LegalEntityType,
            ip.ContributorType,
			CASE 
				WHEN ip.CisacType IN (SELECT RoleTypeID FROM Lookup.RoleType WHERE Code in ('AM', 'MA', 'TA')) THEN CAST(1 AS bit)
				ELSE CAST(0 AS bit)
			END AS IsWriter,
            JSON_QUERY(
                (SELECT 
                    ref.IPNameNumber AS IpNameNumber,
                    ipi.AmendedDateTime,
                    name.FirstName AS FirstName,
                    name.LastName AS LastName,
                    name.CreatedDate,
                    ISNULL(
                        CASE 
                            WHEN name.TypeCode = 'DF' THEN CAST(0 AS int)
                            WHEN name.TypeCode = 'HR' THEN CAST(1 AS int)
                            WHEN name.TypeCode = 'MO' THEN CAST(2 AS int)
                            WHEN name.TypeCode = 'OR' THEN CAST(3 AS int)
                            WHEN name.TypeCode = 'PA' THEN CAST(4 AS int)
                            WHEN name.TypeCode = 'PG' THEN CAST(5 AS int)
                            WHEN name.TypeCode = 'PP' THEN CAST(6 AS int)
                            WHEN name.TypeCode = 'ST' THEN CAST(7 AS int)
                            ELSE NULL
                        END, 99
                    ) AS TypeCode, 
                    name.ForwardingNameNumber,
                    ipi.AgencyID AS Agency
                 FROM IPI.InterestedParty ipi 
                 JOIN IPI.NameReference ref ON ipi.IPBaseNumber = ref.IPBaseNumber
                 JOIN IPI.Name name ON ref.IPNameNumber = name.IPNameNumber
                 WHERE ipi.IPBaseNumber = ip.IPBaseNumber 
                 FOR JSON PATH
                )
            ) AS Names
         FROM (
             SELECT
                c.CreatorID AS ContributorID,
                c.IPBaseNumber, 
                c.IPNameNumber,
                c.RoleTypeID AS CisacType,
                c.Authoritative AS IsAuthoritative,
                c.Status,
                c.FirstName AS Name,
                c.LastName,
                c.DisplayName,
                c.CreatedDate,
                c.IsExcludedFromIswc,
                ipi.AgencyID AS Agency,
                ipi.DeathDate,
                ipi.Type AS LegalEntityType,
                2 AS ContributorType
             FROM ISWC.Creator c
             INNER JOIN IPI.InterestedParty ipi ON c.IPBaseNumber = ipi.IPBaseNumber
             WHERE WorkInfoID = wi.WorkInfoID AND IswcID = wi.IswcID AND c.Status <> 0
             UNION ALL
             SELECT 
                p.PublisherID AS ContributorID,
                p.IPBaseNumber, 
                p.IPNameNumber,
                p.RoleTypeID AS CisacType,
                NULL AS IsAuthoritative,
                p.Status,
                p.FirstName AS Name,
                p.LastName,
                p.DisplayName,
                p.CreatedDate,
                null,
                ipi.AgencyID AS Agency,
                ipi.DeathDate,
                ipi.Type AS LegalEntityType,
                1 AS ContributorType
             FROM ISWC.Publisher p
             INNER JOIN IPI.InterestedParty ipi ON p.IPBaseNumber = ipi.IPBaseNumber
             WHERE WorkInfoID = wi.WorkInfoID AND IswcID = wi.IswcID AND p.Status <> 0
         ) ip
         FOR JSON PATH
        )
    ) AS InterestedParties,
	JSON_QUERY(
        (SELECT 
            p.PerformerID,
            p.Isni,
            p.Ipn,
            p.FirstName,
            p.LastName,
            pd.Code AS Designation
         FROM ISWC.WorkInfoPerformer wip 
         INNER JOIN ISWC.Performer p ON wip.PerformerID = p.PerformerID
		 INNER JOIN Lookup.PerformerDesignation pd on wip.PerformerDesignationID = pd.PerformerDesignationID
         WHERE wip.WorkInfoID = wi.WorkInfoID AND wip.Status <> 0
         FOR JSON PATH
        )
    ) AS Performers
FROM ISWC.ISWC i
INNER JOIN ISWC.WorkInfo wi ON i.IswcID = wi.IswcID
LEFT JOIN Lookup.IswcStatus lis ON i.IswcStatusID = lis.IswcStatusID
WHERE wi.Status <> 0
GO