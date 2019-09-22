/*
Tables: 
Main
Accesscode
*/

USE [APISERVICE_DB]
GO

DROP TABLE IF EXISTS [dbo].[Accesscode]
DROP TABLE IF EXISTS [dbo].[Main]


----------- TABLE Main ---------------

CREATE TABLE [dbo].[Main](
	[mainid] [bigint] IDENTITY(1,1) NOT NULL,
	[session] [uniqueidentifier] DEFAULT NEWID() NOT NULL,
	[created] [datetime] DEFAULT GETDATE() NOT NULL,
	[changed] [datetime] DEFAULT GETDATE() NOT NULL,
	[clsid] [uniqueidentifier] NOT NULL,
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
GO

-- Will usually be very little selective, as it only distincts the serialized classes:
CREATE NONCLUSTERED INDEX [IX_Main_clsid] ON [dbo].[Main]
(
	[clsid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


----------- TABLE Accesscode ---------------

CREATE TABLE [dbo].[Accesscode](
	[accesscodeid] [bigint] IDENTITY(1,1) NOT NULL,
	[session] [uniqueidentifier] DEFAULT NEWID() NOT NULL,
	[created] [datetime] DEFAULT GETDATE() NOT NULL,
	[changed] [datetime] DEFAULT GETDATE() NOT NULL,
	[phonenumber] varchar(25) NOT NULL,
    [accesscode] varchar(6) NOT NULL
 CONSTRAINT [PK_Accesscode] PRIMARY KEY NONCLUSTERED 
(
	[accesscodeid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TRIGGER TRG_Accesscode_changed ON [dbo].[Accesscode] FOR UPDATE AS
	UPDAtE [dbo].[Accesscode]
	SET changed = GETDATE()
	FROM [dbo].[Accesscode] m
	INNER JOIN INSERTED i ON m.[accesscodeid] = i.[accesscodeid]
GO

ALTER TABLE [dbo].[Accesscode]  WITH CHECK ADD  CONSTRAINT [FK_Accesscode_Main] FOREIGN KEY([session])
REFERENCES [dbo].[Main] ([session])
GO

ALTER TABLE [dbo].[Accesscode] CHECK CONSTRAINT [FK_Accesscode_Main]
GO

CREATE NONCLUSTERED INDEX [IX_Accesscode_phonenumber] ON [dbo].[Accesscode]
(
	[phonenumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

