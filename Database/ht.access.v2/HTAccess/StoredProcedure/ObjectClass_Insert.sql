CREATE PROCEDURE [Access].[ObjectClass_Insert](
	@Name NVARCHAR(60),
	@Description NVARCHAR(1000),
	@IsObsolete BIT = 0,
	@IsAbstract BIT = 0,
	@IsStructural BIT = 0,
	@IsAuxiliary BIT = 0,
	@ParentClassName NVARCHAR(60) = NULL
)
AS
BEGIN
	DECLARE @ParentClassId INT
	IF @ParentClassName IS NOT NULL
	BEGIN
		SELECT TOP (1) @ParentClassId = ObjectClassId FROM Access.ObjectClasses WITH (NOLOCK) WHERE [Name] = @ParentClassName
	END
	INSERT INTO Access.ObjectClasses (
		[Name],
		[Description],
		IsObsolete,
		IsAbstract,

		IsStructural,
		IsAuxiliary,
		ParentObjectClassId
	) VALUES (
		@Name,
		@Description,
		@IsObsolete,
		@IsAbstract,

		@IsStructural,
		@IsAuxiliary,
		@ParentClassId
	)

	SELECT cast(SCOPE_IDENTITY() as int) 'ObjectClassId'


END
	
