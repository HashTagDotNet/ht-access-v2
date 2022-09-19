CREATE TABLE [Access].[ObjectAttributes]
(
	[ObjectAttributeId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ObjectId] INT NOT NULL,
    [Attribute] NVARCHAR(60) NULL, 
    [Value] NVARCHAR(MAX) NULL, 
    [Value_Hash] Binary(64) NULL,
    CONSTRAINT [FK_ObjectAttributes_Objects] FOREIGN KEY (ObjectId) REFERENCES [Access].[Objects]([ObjectId])
)

