CREATE TABLE [Access].[ObjectClasses]
(
	[ObjectClassId] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] nvarchar(60) NOT NULL,
	[Description] nvarchar(1000),
	[IsObsolete] bit default 0 NOT NULL,
	[IsAbstract] bit default 0 NOT NULL,
	[IsStructural] bit default 1 NOT NULL,
	[IsAuxiliary] bit default 0 NOT NULL,
	[ParentObjectClassId] int default NULL, 
    CONSTRAINT [FK_ObjectClasses_ObjectClasses] FOREIGN KEY ([ParentObjectClassId]) REFERENCES [Access].[ObjectClasses]([ObjectClassId])

)

GO

CREATE UNIQUE INDEX [AK_ObjectClasses_Name] ON [Access].[ObjectClasses] ([Name])
