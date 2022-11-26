CREATE PROCEDURE [Access].[Attribute_DeleteByName](
	@Name NVARCHAR(60)
)
AS
BEGIN
	DELETE TOP(1)
	FROM
		Access.Attributes
	WHERE
		[Name] = @Name
END
	
