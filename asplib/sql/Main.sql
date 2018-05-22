/*
Database storage for Main objects as alternative to viewstate or session
*/

USE [ASP_DB]
GO

CREATE TABLE [dbo].[Main](
	[mainid] [bigint] IDENTITY(1,1) NOT NULL,
	[session] [uniqueidentifier] NOT NULL,
	[created] [datetime] NOT NULL,
	[changed] [datetime] NOT NULL,
	[main] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Main] PRIMARY KEY NONCLUSTERED 
(
	[session] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [CIX_Main_mainid] ON [dbo].[Main]
(
	[mainid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

