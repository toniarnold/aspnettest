/*
Database storage for Main objects as alternative to viewstate or session
Uses separate IDENTITY int and PRIMARY KEY guid columns, which are are
(like the time stamps) handled by the database itself.
*/

USE [ASP_DB]
GO

DROP TABLE IF EXISTS [dbo].[Main]

CREATE TABLE [dbo].[Main](
	[mainid] [bigint] IDENTITY(1,1) NOT NULL,
	[session] [uniqueidentifier] DEFAULT NEWID() NOT NULL,
	[created] [datetime] DEFAULT GETDATE() NOT NULL,
	[changed] [datetime] DEFAULT GETDATE() NOT NULL,
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

CREATE TRIGGER TRG_Main_changed ON [dbo].[Main] FOR UPDATE AS
	UPDAtE [dbo].[Main]
	SET changed = GETDATE()
	FROM [dbo].[Main] m
	INNER JOIN INSERTED i ON m.mainid = i.mainid
