USE [ETL]
GO

/****** Object:  UserDefinedFunction [dbo].[uf_CheckInternalSystems]    Script Date: 12-Aug-20 01:09:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[uf_CheckInternalSystems]
(
	@SystemId INT
)
RETURNS BIT
BEGIN
	RETURN (SELECT COUNT(1) FROM InternalSystems WHERE Id = @SystemId)
END

GO


