CREATE PROCEDURE [Access].[ObjectClass_ExistsByName](
	@Name NVARCHAR(60)
) AS
BEGIN
	DECLARE @ObjectClassId INT
	SELECT TOP(1) @ObjectClassId = ObjectClassId FROM Access.ObjectClasses WITH (NOLOCK) WHERE [NAME] = @Name;
	SELECT CAST(IIF(@ObjectClassId IS NULL, 0,1) AS BIT) ObjectExists
END
