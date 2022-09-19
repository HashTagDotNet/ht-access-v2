CREATE TABLE [dbo].[ObjectAttributes]
(
	[ObjectAttributeId] INT NOT NULL PRIMARY KEY, 
    [ObjectId] INT NOT NULL,
    [AttributeName] NVARCHAR(60) NULL, 
    [AttributeValue] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_ObjectAttributes_Objects] FOREIGN KEY (ObjectId) REFERENCES [Access].[Objects]([ObjectId])
)
