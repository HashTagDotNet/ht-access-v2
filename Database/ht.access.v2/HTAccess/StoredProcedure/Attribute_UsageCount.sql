CREATE PROCEDURE [Access].[Attribute_IsUsed] (
	@Name NVARCHAR(60)
) AS
BEGIN
	DECLARE @ObjectClassCount INT
	DECLARE @EntryCount INT
	DECLARE @AttributeId INT
	SELECT TOP 1 @AttributeId = AttributeId FROM Access.Attributes WITH (NOLOCK) WHERE [Name] = @Name
	IF @AttributeId IS NULL
	BEGIN
		RETURN NULL
	END

	SELECT TOP(1) @EntryCount = 1 FROM Access.EntryAttributes WITH (NOLOCK) WHERE AttributeId = @AttributeId
	SELECT TOP(1) @ObjectClassCount = 1 FROM Access.ObjectClassAttributes WITH (NOLOCK) WHERE AttributeId = @AttributeId

	SELECT coalesce(@EntryCount,0) + coalesce(@ObjectClassCount,0) 'UsageFlag'
	
END
