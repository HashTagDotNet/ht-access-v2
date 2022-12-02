CREATE PROCEDURE [Access].[Attribute_ExistsByName](
	@Name NVARCHAR(60)
) AS
BEGIN
	DECLARE @AttributeId INT
	SELECT TOP(1) @AttributeId = AttributeId FROM Access.Attributes WITH (NOLOCK) WHERE [NAME] = @Name;
	SELECT CAST(IIF(@AttributeId IS NULL, 0,1) AS BIT) AttributeExists
END
	