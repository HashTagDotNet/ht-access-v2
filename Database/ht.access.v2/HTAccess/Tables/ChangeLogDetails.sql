CREATE TABLE [Access].[ChangeLogDetails]
(
	[ChangeLogDetailId] INT NOT NULL PRIMARY KEY IDENTITY,
	[ChangeLogId] INT NOT NULL, 
    [CommandKey] VARCHAR(100) NOT NULL, 
    [CommandValue] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_ChangeLogDetails_ChangeLog] FOREIGN KEY ([ChangeLogId]) REFERENCES [Access].[ChangeLog]([ChangeLogId])
)
