CREATE TABLE [Access].[ChangeLog]
(
	[ChangeLogId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DN] NVARCHAR(MAX) NOT NULL, 
    [ChangeType] varchar(20) NOT NULL, 
    [StatusCode] varchar(20) NOT NULL,
    [Message] nvarchar(max) NULL,
    [ChangeDate] DATETIME2 NOT NULL DEFAULT getutcdate()
)
