﻿CREATE TABLE [Access].[Attributes]
(
	[AttributeId] INT NOT NULL PRIMARY KEY IDENTITY, 
	[AttributeValueTypeId] INT NOT NULL DEFAULT 0,
	[IsObsolete] bit NOT NULL DEFAULT 0,
	[Name] NVARCHAR(60) NOT NULL,
	[Description] nvarchar(1000),
	[AllowMultipleValues] bit NOT NULL DEFAULT 1,
	[AllowUserModification] bit NOT NULL DEFAULT 1, 
    [Alias] NVARCHAR(60) NULL, 
    CONSTRAINT [FK_AttributeValueTypes] FOREIGN KEY ([AttributeValueTypeId]) REFERENCES [Access].[AttributeValueTypes]([AttributeValueTypeId]),
)

GO

CREATE UNIQUE INDEX [AFK_Attributes_Name] ON [Access].[Attributes] ([Name])
