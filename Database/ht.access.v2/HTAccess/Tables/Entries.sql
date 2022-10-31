CREATE TABLE [Access].[Entries](
	[EntryId] [bigint] IDENTITY(1,1) NOT NULL,
	
	
	[DN] [nvarchar](max) NOT NULL,
	[DN_Hash] [binary](64) NOT NULL,

    [UpdatedOn] DATETIME NOT NULL DEFAULT getutcdate(), 
    CONSTRAINT [PK_Entries] PRIMARY KEY ([EntryId]),
 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE UNIQUE INDEX [AK_Entries_Dn] ON [Access].[Entries] ([DN_Hash])
