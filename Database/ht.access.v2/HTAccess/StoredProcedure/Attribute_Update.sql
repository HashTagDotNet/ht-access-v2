CREATE PROCEDURE [Access].[Attribute_Update](
	@Name NVARCHAR(60),
	@ValueTypeName NVARCHAR(20),
	@IsObsolete BIT = 0,	
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
	UPDATE TOP(1)
		[Access].[Attributes]
	SET
		AttributeValueTypeId = @ValueTypeId,
		IsObsolete = @IsObsolete,
		[Description] = @Description,
		AllowMultipleValues = @AllowMultipleValues,

		@AllowUserModification = @AllowUserModification
	WHERE
		[Name] = @Name
END