USE [ETL]
GO
/****** Object:  StoredProcedure [dbo].[up_FindRunningQueries]    Script Date: 12-Aug-20 06:59:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--up_FindRunningQueries @SPID = null, @IncludeCallingSession = 0
ALTER PROCEDURE [dbo].[up_FindRunningQueries]
	@SPID VARCHAR(10)
	,@IncludeCallingSession BIT
AS
BEGIN
	;WITH cteRunningSPIDs AS
	(
		select
			spid AS [Session Id]
			,RIGHT(CONVERT(VARCHAR, DATEADD(MILLISECOND, DATEDIFF(MILLISECOND, last_batch, GETDATE()), '1900-01-01'), 121), 12) AS [Batch Duration]
			,hostname AS [Calling Machine]
			,hostprocess AS [Calling Process Id]
			,cmd AS [Command Type]
			,[dbid] AS [Database Id]
			,blocked AS [Is Process Blocked]
			,nt_username AS [Network Username]
			,open_tran AS [Open Transaction]
			,[status] AS [Query Status]
			,[program_name] AS [Running Program]
			,loginame AS [Sql Login]
		FROM sys.sysprocesses
		-- ignore system functions and non-running sessions
		WHERE [status] NOT IN ('background','sleeping')
			AND cmd NOT IN ('awaiting command''mirror handler','lazy writer','checkpoint sleep','ra manager')
			-- if a SPID is passed, ensure it is in the results
			AND spid LIKE (CASE WHEN @SPID <> NULL THEN @SPID ELSE '%' END)
			-- if the calling SPID is not to be used, do not include it
			AND spid NOT LIKE (CASE WHEN @IncludeCallingSession = 0 THEN CONVERT(VARCHAR, @@SPID) ELSE '' END)
	)
	SELECT
		spids.[Session Id]
		,spids.[Batch Duration]
		,spids.[Calling Machine]
		,spids.[Calling Process Id]
		,dbs.name AS [Database Name]
		,spids.[Is Process Blocked]
		,spids.[Network Username]
		,spids.[Open Transaction]
		,spids.[Query Status]
		,(SELECT [text] FROM sys.dm_exec_sql_text(reqs.[sql_handle])) AS [Query Text]
		,spids.[Running Program]
		,spids.[Sql Login]
	FROM sys.databases dbs
	INNER JOIN cteRunningSPIDs spids
		ON dbs.database_id = spids.[Database Id]
	INNER JOIN sys.dm_exec_requests reqs
		ON spids.[Session Id] = reqs.session_id
END
