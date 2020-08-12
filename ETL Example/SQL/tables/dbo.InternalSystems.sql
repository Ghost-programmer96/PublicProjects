USE [ETL]
GO

/****** Object:  Table [dbo].[InternalSystems]    Script Date: 12-Aug-20 01:07:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[InternalSystems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SystemDescription] [varchar](1000) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


