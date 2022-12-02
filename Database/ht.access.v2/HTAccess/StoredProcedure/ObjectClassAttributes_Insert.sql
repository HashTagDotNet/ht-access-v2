CREATE PROCEDURE [Access].[ObjectClassAttributes_Insert](
	@ObjectClassId INT,
	@Name NVARCHAR(60),
	@IsRequired BIT
) AS
BEGIN
	DECLARE @AttributeId INT
	SELECT TOP (1) @AttributeId = AttributeId FROM Access.Attributes WITH (NOLOCK) WHERE [Name] = @Name
	INSERT INTO Access.ObjectClassAttributes (
		ObjectClassId,
		AttributeId,
		IsRequired
	) VALUES (
		@ObjectClassId,
		@AttributeId,
		@IsRequired
	)
END
