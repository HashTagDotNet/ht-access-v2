CREATE TABLE [Access].[Objects](
	[ObjectId] [int] IDENTITY(1,1) NOT NULL,
	[ObjectUid] [char](32) NOT NULL,
	[DomainId] [int] NULL,
	
	[DN] [nvarchar](max) NOT NULL,
	[DN_Hash] [binary](64) NOT NULL,

	[RDN] [nvarchar](100) NULL,	
	[RDN_Hash] [binary](64) NULL,
	[RDN_Attribute] [nvarchar](60) NULL,
	[RDN_Value] [nvarchar](100) NULL, 

	[Parent_DN_Id] [int] NULL,
	[Parent_DN] NVARCHAR(max) NULL,
	[Parent_DN_Hash] [binary](64) NULL,

    CONSTRAINT [PK_Objects] PRIMARY KEY ([ObjectId]),
	CONSTRAINT [FK_Objects_ToObjects] FOREIGN KEY ([Parent_DN_ID]) REFERENCES [Access].[Objects]([ObjectId]),    
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Access].[Objects] ADD  CONSTRAINT [DF_Objects_DirectoryEntryUid]  DEFAULT (lower(replace(CONVERT([varchar](50),newid()),'-',''))) FOR [ObjectUid]
GO