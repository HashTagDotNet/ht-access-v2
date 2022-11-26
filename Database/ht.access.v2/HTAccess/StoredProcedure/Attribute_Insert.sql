CREATE PROCEDURE [Access].[Attribute_Insert](
	@ValueTypeName NVARCHAR(20),
	@IsObsolete BIT = 0,
	@Name NVARCHAR(60),
	@Description NVARCHAR(1000),
	@AllowMultipleValues BIT = 1,
	@AllowUserModification BIT = 1
) AS
BEGIN
	DECLARE @ValueTypeId INT
	SELECT TOP(1) @ValueTypeId = AttributeValueTypeId FROM Access.AttributeValueTypes WITH (NOLOCK) WHERE [Type]=@ValueTypeName;
	IF @ValueTypeId IS NULL 
	BEGIN
		RETURN
	END
	INSERT INTO Access.Attributes (
		AttributeValueTypeId,
		IsObsolete,
		[Name],
		[Description],

		AllowMultipleValues,
		AllowUserModification
	) VALUES (
		@ValueTypeId,
		@IsObsolete,
		@Name,
		@Description,

		@AllowMultipleValues,
		@AllowUserModification
	)
END
