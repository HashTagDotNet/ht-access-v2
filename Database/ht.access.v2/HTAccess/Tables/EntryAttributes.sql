CREATE TABLE [Access].[EntryAttributes]
(
	[EntryAttributeId] [bigint] IDENTITY(1,1) NOT NULL,
	[EntryId] [bigint] NOT NULL,
	[AttributeId] [int] NULL,
	[StringValue] [nvarchar](max) NULL,
	[StringValue_Hash] [binary](64) NULL,
	[NumberValue] [float] NULL,
	[DateValue] [datetime2](7) NULL,
	[LinkValue] [bigint] NULL,
	[BooleanValue] [bit] NULL,
    CONSTRAINT [FK_EntryAttributes_Objects] FOREIGN KEY (EntryId) REFERENCES [Access].[Entries]([EntryId]),
	CONSTRAINT [FK_EntryAttributes_ObjectClassAttributes] FOREIGN KEY (AttributeId) REFERENCES [Access].[ObjectClassAttributes]([ObjectClassAttributeId]),
)

