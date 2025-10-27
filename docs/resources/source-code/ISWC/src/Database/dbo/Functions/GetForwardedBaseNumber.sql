CREATE FUNCTION [dbo].[GetForwardedBaseNumber]
(	
	@IPBaseNumber nvarchar(13)
)
RETURNS TABLE 
AS
RETURN 
(
	with IPBaseNumberCTE
	as 
	(
	select IPBaseNumber, ForwardingBaseNumber, 0 as relative_depth from IPI.Status
	where IPBaseNumber = @IPBaseNumber
	and ToDate > CURRENT_TIMESTAMP

	union all

	select s.IPBaseNumber, s.ForwardingBaseNumber, cte.relative_depth + 1 from IPI.Status s
	inner join IPBaseNumberCTE cte on cte.ForwardingBaseNumber = s.IPBaseNumber
	where cte.IPBaseNumber <> cte.ForwardingBaseNumber
	and ToDate > CURRENT_TIMESTAMP
	)
	select top 1 ForwardingBaseNumber from IPBaseNumberCTE
	order by relative_depth desc
)