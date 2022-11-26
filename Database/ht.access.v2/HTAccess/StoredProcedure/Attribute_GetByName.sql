CREATE PROCEDURE [Access].[Attribute_GetByName](
	@Name NVARCHAR(60)
)
AS
BEGIN
	SELECT TOP(1)
		A.*,
		AT.[Type]
	FROM
		Access.Attributes A WITH (NOLOCK)
		JOIN Access.AttributeValueTypes AT WITH (NOLOCK)
			ON A.AttributeValueTypeId = AT.AttributeValueTypeId
	WHERE
		[Name] = @Name
END
	
