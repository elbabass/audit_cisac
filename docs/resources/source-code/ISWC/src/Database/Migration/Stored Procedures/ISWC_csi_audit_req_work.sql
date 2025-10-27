



-- =============================================
-- Author: Dylan Mac Namee
-- Create Date: 09/07/2019
-- Description: 
-- =============================================

CREATE PROCEDURE [Migration].[ISWC_csi_audit_req_work]
(
   @Audit Migration.Azure_ISWC_csi_audit_req_work READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [ISWC].[csi_audit_req_work]
	([titles],
	[ips],
	[created_dt],
	[soccde],
	[socwrkcde],
	[srcdb],
	[update_seq],
	[last_updated_dt],
	[last_updated_user],
	[req_tx_id],
	[arch_iswc],
	[pref_iswc],
	[cat],
	[deleted],
	[posted_dt],
	[lst_updated_dt]
	)
	SELECT 
		a.titles, a.ips, a.created_dt, a.soccde, a.socwrkcde, a.srcdb, a.update_seq, a.last_updated_dt, 1,
		a.req_tx_id, a.arch_iswc, a.pref_iswc, a.cat, a.deleted, a.posted_dt, a.lst_updated_dt
	FROM @Audit a
	--JOIN Lookup.[User] u on a.last_updated_user = u.Name

END