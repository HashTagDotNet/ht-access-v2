CREATE TABLE [Access].[EntryObjectClasses]
(
	[EntryObjectClassId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EntryId] BIGINT NULL, 
    [ObjectClassId] int NULL, 
    CONSTRAINT [FK_EntryObjectClasses_ObjectClasses] FOREIGN KEY ([ObjectClassId]) REFERENCES [Access].[ObjectClasses]([ObjectClassId]), 
    CONSTRAINT [FK_EntryObjectClasses_Entries] FOREIGN KEY ([EntryId]) REFERENCES [Access].[Entries]([EntryId])
)
