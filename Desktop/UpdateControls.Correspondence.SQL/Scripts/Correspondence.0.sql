IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Correspondence_Version]') AND type in (N'U'))
CREATE TABLE dbo.Correspondence_Version
	(
	VersionId int IDENTITY(1,1) NOT NULL,
	Version int NOT NULL,
	CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED 
	(
		[VersionID] ASC
	)
	)

IF NOT EXISTS (SELECT * FROM Correspondence_Version)
INSERT INTO Correspondence_Version (Version) VALUES (0)
