USE [ETL]
GO

/****** Object:  Table [dbo].[Accounts_Loading]    Script Date: 12-Aug-20 01:07:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Accounts_Loading](
	[AccountNumber] [varchar](100) NOT NULL,
	[InternalSystemId] [int] NOT NULL,
	[PatientFirstName] [varchar](100) NOT NULL,
	[PatientLastName] [varchar](100) NOT NULL,
	[GuarantorFirstName] [varchar](100) NOT NULL,
	[GuarantorLastName] [varchar](100) NOT NULL,
	[InitialBalance] [decimal](18, 2) NOT NULL,
	[Adjustments] [decimal](18, 2) NOT NULL,
	[Charges] [decimal](18, 2) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Accounts_Loading]  WITH NOCHECK ADD  CONSTRAINT [CHK__Accounts_Loading__InternalSystemId] CHECK  (([dbo].[uf_CheckInternalSystems]([InternalSystemId])=(1)))
GO

ALTER TABLE [dbo].[Accounts_Loading] CHECK CONSTRAINT [CHK__Accounts_Loading__InternalSystemId]
GO


