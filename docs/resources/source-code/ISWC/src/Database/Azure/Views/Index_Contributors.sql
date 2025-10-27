CREATE VIEW [Azure].[Index_Contributors]
AS
select 
	ip.IPBaseNumber + cast(name.IPNameNumber as nvarchar) as GeneratedID,
	name.FirstName as FirstName,
	name.LastName as LastName,
	isnull(name.FirstName,'') + ' ' + isnull(name.LastName,'') as FullName,
	name.TypeCode as Type,
	name.IPNameNumber as IPINumber,
	null as IPCode,
	ip.IPBaseNumber as IPBaseNumber,
	ip.Concurrency as Concurrency,
	cast(0 as bit) as IsDeleted,
	ip.Type as [LegalEntityType]
from IPI.InterestedParty ip 
join IPI.NameReference ref on ip.IPBaseNumber = ref.IPBaseNumber
join IPI.Name name on ref.IPNameNumber = name.IPNameNumber