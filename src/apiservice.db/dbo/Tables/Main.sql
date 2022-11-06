CREATE TABLE [dbo].[Main] (
    [mainid]  BIGINT           IDENTITY (1, 1) NOT NULL,
    [session] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [created] DATETIME         DEFAULT (getdate()) NOT NULL,
    [changed] DATETIME         DEFAULT (getdate()) NOT NULL,
    [clsid]   UNIQUEIDENTIFIER NOT NULL,
    [main]    VARBINARY (MAX)  NOT NULL,
    CONSTRAINT [PK_Main] PRIMARY KEY NONCLUSTERED ([session] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [CIX_Main_mainid]
    ON [dbo].[Main]([mainid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Main_clsid]
    ON [dbo].[Main]([clsid] ASC);


GO

CREATE TRIGGER TRG_Main_changed ON [dbo].[Main] FOR UPDATE AS
	UPDAtE [dbo].[Main]
	SET changed = GETDATE()
	FROM [dbo].[Main] m
	INNER JOIN INSERTED i ON m.mainid = i.mainid
