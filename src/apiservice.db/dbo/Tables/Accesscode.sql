CREATE TABLE [dbo].[Accesscode] (
    [accesscodeid] BIGINT           IDENTITY (1, 1) NOT NULL,
    [session]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [created]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [changed]      DATETIME         DEFAULT (getdate()) NOT NULL,
    [phonenumber]  VARCHAR (25)     NOT NULL,
    [accesscode]   VARCHAR (6)      NOT NULL,
    CONSTRAINT [PK_Accesscode] PRIMARY KEY NONCLUSTERED ([accesscodeid] ASC),
    CONSTRAINT [FK_Accesscode_Main] FOREIGN KEY ([session]) REFERENCES [dbo].[Main] ([session])
);


GO
CREATE NONCLUSTERED INDEX [IX_Accesscode_phonenumber]
    ON [dbo].[Accesscode]([phonenumber] ASC);


GO

CREATE TRIGGER TRG_Accesscode_changed ON [dbo].[Accesscode] FOR UPDATE AS
	UPDAtE [dbo].[Accesscode]
	SET changed = GETDATE()
	FROM [dbo].[Accesscode] m
	INNER JOIN INSERTED i ON m.[accesscodeid] = i.[accesscodeid]
