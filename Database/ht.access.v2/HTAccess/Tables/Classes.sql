CREATE TABLE [Access].[Classes]
(
	[ClassId] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] varchar(60) NOT NULL,
	[Description] varchar(500),
	[IsObsolete] bit default 0 NOT NULL,
	[IsAbstract] bit default 0 NOT NULL,
	[IsStructural] bit default 1 NOT NULL,
	[IsAuxiliary] bit default 0 NOT NULL,
	[MustAttributes] varchar(max),
	[MayAttributes] varchar(max),
	[ParentClassId] int default NULL, 
    CONSTRAINT [FK_Classes_Classes] FOREIGN KEY ([ParentClassId]) REFERENCES [Access].[Classes]([ClassId])

)

GO

CREATE UNIQUE INDEX [AK_ObjectClasses_Name] ON [Access].[Classes] ([Name])
