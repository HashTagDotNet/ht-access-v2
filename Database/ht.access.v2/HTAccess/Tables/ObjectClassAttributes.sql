CREATE TABLE [Access].[ObjectClassAttributes]
(
	[ObjectClassAttributeId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ObjectClassId] INT NOT NULL, 
    [AttributeId] INT NOT NULL, 
    [IsRequired] BIT NOT NULL DEFAULT 0
)
