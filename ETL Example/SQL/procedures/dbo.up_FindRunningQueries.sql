USE [ETL]
GO

/****** Object:  StoredProcedure [dbo].[up_FindRunningQueries]    Script Date: 12-Aug-20 01:08:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[up_FindRunningQueries]
	@SPID varchar(10)
as
begin
	declare @SqlHandle varbinary(64) = (select sql_handle from etl.sys.sysprocesses where spid = @SPID)

	;with cteSysProcesses as
	(
		select
			right(convert(varchar, dateadd(ms, datediff(ms, last_batch, getdate()), '1900-01-01'), 121), 12) as [Batch Duration]
			,hostname as [Calling Machine]
			,hostprocess as [Calling Process Id]
			,cmd as [Command Type]
			,[dbid] as [Database Id]
			,blocked as [Is Process Blocked]
			,nt_username as [Network Username]
			,open_tran as [Open Transaction]
			,[status] as [Query Status]
			,[program_name] as [Running Program]
			,@SqlHandle as [sql_handle]
			,loginame as [Sql Login]
		from sys.sysprocesses
		where spid = @SPID
	)
	,cteSqlText as
	(
		select
			@SqlHandle as [sql_handle]
			,[text] as [Query Text]
		from sys.dm_exec_sql_text(@SqlHandle)
	), cteDatabases as
	(
		select
			name as [Database Name]
			,database_id as [Database Id]
		from sys.databases
	)
	select
		cteSysProcesses.[Batch Duration]
		,cteSysProcesses.[Calling Machine]
		,cteSysProcesses.[Calling Process Id]
		,cteSysProcesses.[Command Type]
		,cteDatabases.[Database Name]
		,cteSysProcesses.[Is Process Blocked]
		,cteSysProcesses.[Network Username]
		,cteSysProcesses.[Open Transaction]
		,cteSysProcesses.[Query Status]
		,cteSqlText.[Query Text]
		,cteSysProcesses.[Running Program]
		,cteSysProcesses.[Sql Login]
	from cteSqlText
	inner join cteSysProcesses
		on cteSqlText.sql_handle = cteSysProcesses.sql_handle
	inner join cteDatabases
		on cteSysProcesses.[Database Id] = cteDatabases.[Database Id]
	where cteSysProcesses.[Query Status] not in ('background','sleeping')
		and cteSysProcesses.[Command Type] not in ('awaiting command''mirror handler','lazy writer','checkpoint sleep','ra manager')
end

GO


