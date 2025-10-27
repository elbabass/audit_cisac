CREATE PROCEDURE [dbo].[TransferCreators]
WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	

	--- Set parameters 
	DECLARE @BatchSize INT = 250000;
	DECLARE @batchRowCount INT;
	DECLARE @batchcount INT = 0;
	DECLARE @OverallStartTime DATETIME = SYSDATETIME();

	DECLARE @BeginMessage varchar(max) = ('Begin Script and insert to [temp.CreatorsTransfer]' + ' - Time: ' + CONVERT(varchar, SYSDATETIME(), 121) + ' - Batch Size: ' + CONVERT(varchar, @BatchSize, 121) );
	RAISERROR (@BeginMessage, 0, 1) WITH NOWAIT;

	-- Commented Out - Initial Temp Table Insert Done 
	--IF OBJECT_ID('[temp.CreatorsTransfer]') IS NOT NULL 
	--BEGIN 
	--	DROP TABLE [temp.CreatorsTransfer]
	--END
	--CREATE TABLE [temp.CreatorsTransfer](
	--	[IPBaseNumber] [nvarchar](13),
	--	[WorkInfoID] [bigint],
	--	[Status] [bit],
	--	[CreatedDate] [datetime2](0),
	--	[LastModifiedDate] [datetime2](0),
	--	[LastModifiedUserID] [int],
	--	[IsDispute] [bit],
	--	[Authoritative] [bit],
	--	[RoleTypeID] [int],
	--	[IswcID] [bigint],
	--	[IPNameNumber] [int],
	--	[ID] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL
	--);

	--INSERT INTO [temp.CreatorsTransfer] (IPBaseNumber, 
	--WorkInfoID,
	--Status,
	--CreatedDate,
	--LastModifiedDate,	
	--LastModifiedUserID,
	--IsDispute,
	--Authoritative,
	--RoleTypeID,
	--IswcID,
	--IPNameNumber)
	--SELECT
	--	DISTINCT IPBaseNumber,
	--		WorkInfoID,
	--		Status,
	--		CreatedDate,
	--		LastModifiedDate,
	--		LastModifiedUserID,
	--		IsDispute,
	--		Authoritative,
	--		RoleTypeID,
	--		IswcID,
	--		IPNameNumber
	--FROM
	--	ISWC.Creator_BACKFILL;
	-- -- 93143657  Rows


	IF OBJECT_ID('tempdb..#Temp_BatchProcessRows') IS NOT NULL 
	BEGIN 
		DROP TABLE #Temp_BatchProcessRows 
	END
	CREATE TABLE #Temp_BatchProcessRows ([IPBaseNumber] [nvarchar](13),
		[WorkInfoID] [bigint],
		[Status] [bit],
		[CreatedDate] [datetime2](0),
		[LastModifiedDate] [datetime2](0),
		[LastModifiedUserID] [int],
		[IsDispute] [bit],
		[Authoritative] [bit],
		[RoleTypeID] [int],
		[IswcID] [bigint],
		[IPNameNumber] [int]
	);
	
	DECLARE @InsertTo varchar(max) = ('Initial Bulk Insert to temp.CreatorsTransfer done! - Time: ' + CONVERT(varchar, SYSDATETIME(), 121));
	RAISERROR (@InsertTo, 0, 1) WITH NOWAIT;


	-- Initial Total Remaining so that we enter the loop
	DECLARE @totalRemaining int = (SELECT COUNT(ID) FROM [temp.CreatorsTransfer])

	--DECLARE @msg3 varchar(max) = ('About to enter loop. Total Remaining: ' + CONVERT(varchar, @totalRemaining, 121) + ' Transaction Count: ' + CONVERT(varchar, @@TRANCOUNT, 8));
	--RAISERROR (@msg3, 0, 1) WITH NOWAIT;

	-- START WHILE
	WHILE (@totalRemaining > 0)
	BEGIN
		SET @batchcount = @batchcount + 1
		DECLARE @BeginTime DATETIME = SYSDATETIME();
		DECLARE @Flag1 DATETIME = SYSDATETIME();
		DECLARE @Start varchar(max) = ( 'Get Batch from Temp - Batch: ' + CONVERT(varchar, @batchcount, 121)  + ' [Rows Remaining: ' + CONVERT(varchar, @totalRemaining, 121) + '] - Time: ' + CONVERT(varchar, SYSDATETIME(), 121) + '  ELAPSED  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@BeginTime, @Flag1),0), 108) + '  Section Time Taken  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@BeginTime, @Flag1),0), 108) );
		RAISERROR (@Start, 0, 1) WITH NOWAIT;

		BEGIN TRAN CreatorsTransfer

		TRUNCATE TABLE #Temp_BatchProcessRows;
			
		DELETE TOP (@BatchSize) --(@BatchSize) 
		FROM [temp.CreatorsTransfer]
		OUTPUT deleted.IPBaseNumber, 
		deleted.WorkInfoID,
		deleted.Status,
		deleted.CreatedDate,
		deleted.LastModifiedDate,	
		deleted.LastModifiedUserID,
		deleted.IsDispute,
		deleted.Authoritative,
		deleted.RoleTypeID,
		deleted.IswcID,
		deleted.IPNameNumber
		INTO #Temp_BatchProcessRows

		SET @totalRemaining = @totalRemaining - @BatchSize
		
		DECLARE @Flag2 DATETIME = SYSDATETIME();
		DECLARE @EndTempCreator varchar(max) = ('Insert To Key Table Begin - Batch: ' + CONVERT(varchar, @batchcount, 121)  + '- Time: ' + CONVERT(varchar, SYSDATETIME(), 121)  + ' *** TOTAL BATCH TIME ELAPSED ***  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@BeginTime, @Flag2),0), 108)  + '  Section Time Taken  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@Flag1, @Flag2),0), 108) );
		RAISERROR (@EndTempCreator, 0, 1) WITH NOWAIT;


		INSERT INTO [ISWC].[Creator] (IPBaseNumber, WorkInfoID, Status, CreatedDate, LastModifiedDate, LastModifiedUserID, IsDispute, Authoritative, RoleTypeID, IswcID, IPNameNumber)
		SELECT IPBaseNumber, WorkInfoID, Status, CreatedDate, LastModifiedDate, LastModifiedUserID, IsDispute, Authoritative, RoleTypeID, IswcID, IPNameNumber
		FROM #Temp_BatchProcessRows;

		DECLARE @Flag3 DATETIME = SYSDATETIME();
		DECLARE @EndUpdateCreator varchar(max) = ('Insert Complete - Commit Begin - Batch: ' + CONVERT(varchar, @batchcount, 121)  + '- Time: ' + CONVERT(varchar, SYSDATETIME(), 121)  + ' *** TOTAL BATCH TIME ELAPSED ***  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@BeginTime, @Flag3),0), 108)  + '  Section Time Taken  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@Flag2, @Flag3),0), 108) );
		RAISERROR (@EndUpdateCreator, 0, 1) WITH NOWAIT;

		COMMIT TRAN CreatorsTransfer;

		DECLARE @Flag4 DATETIME = SYSDATETIME();
		DECLARE @CommitDone varchar(max) = ('Commit Done - Batch: ' + CONVERT(varchar, @batchcount, 121)  + ' *#* TOTAL TIME SO FAR *#*  :  ' + CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,@OverallStartTime, @Flag4),0), 108)  );
		RAISERROR (@CommitDone, 0, 1) WITH NOWAIT;
		
		
	END

END