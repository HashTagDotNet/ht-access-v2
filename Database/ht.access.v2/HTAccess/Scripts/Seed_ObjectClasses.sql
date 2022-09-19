USE [HT.Access.v2]
GO
DROP PROC IF EXISTS #Seed_ObjectClasses
GO
CREATE PROC #Seed_ObjectClasses(
	@Name varchar(60),
	@ParentClassName varchar(60) = NULL,
	@Description varchar(500) = NULL,
	@IsObsolete bit = 0,
	@IsAbstract bit = 0,
	@IsStructural bit = 1,
	@IsAuxiliary bit = 0,
	@MustAttributes varchar(max) = NULL,
	@MayAttributes varchar(max) = NULL
) AS
BEGIN
	DECLARE @ClassId INT
	DECLARE @ParentClassId INT
	SELECT TOP(1) @ClassId = ClassId FROM Access.ObjectClasses WITH (NOLOCK) WHERE [Name] = @Name
	IF @ParentClassName IS NOT NULL
	BEGIN
		SELECT TOP(1) @ParentClassId =  ClassId FROM Access.ObjectClasses WITH (NOLOCK) WHERE [Name] = @ParentClassName
	END
	IF @ClassId IS NULL
	BEGIN
		INSERT INTO Access.ObjectClasses (
			[Name],
			[Description],
			IsObsolete,
			IsAbstract,
			IsStructural,
			IsAuxiliary,
			MustAttributes,
			MayAttributes,
			ParentClassId
		) VALUES (
			lower(@Name),
			@Description,
			@IsObsolete,
			@IsAbstract,
			@IsStructural,
			@IsAuxiliary,
			@MustAttributes,
			@MayAttributes,
			@ParentClassId
		)
	END
	ELSE
	BEGIN
		UPDATE TOP (1)
			Access.ObjectClasses
		SET
			[Name] = LOWER(@Name),
			[Description] = @Description,
			IsObsolete = @IsObsolete,
			IsAbstract = @IsAbstract,
			IsStructural = @IsStructural,
			IsAuxiliary = @IsAuxiliary,
			MustAttributes = @MustAttributes,
			MayAttributes = @MayAttributes,
			ParentClassId = @ParentClassId
		WHERE
			ClassId = @ClassId
	END
END
GO
BEGIN TRANSACTION

EXEC #Seed_ObjectClasses @Name = 'top',
	@ParentClassName=NULL,
	@Description='Base object from which all other objects inherit',
	@IsObsolete=0,
	@MustAttributes='objectClass',
	@MayAttributes=null,
	@IsAbstract=1,
	@IsStructural=0,
	@IsAuxiliary=0
EXEC #Seed_ObjectClasses @Name = 'domain',
	@ParentClassName='top',
	@Description='The root domain',
	@IsObsolete=0,
	@MustAttributes='do',
	@MayAttributes='code, description',
	@IsAbstract=0,
	@IsStructural=1,
	@IsAuxiliary=0
EXEC #Seed_ObjectClasses @Name = 'meta',
	@ParentClassName='top',
	@Description='Common meta data associated with the object',
	@IsObsolete=0,
	@MustAttributes=null,
	@MayAttributes='code, description, lastupdate',
	@IsAbstract=0,
	@IsStructural=0,
	@IsAuxiliary=1

EXEC #Seed_ObjectClasses @Name = 'application',
	@ParentClassName='domain',
	@Description='Application that belongs to a specific domain',
	@IsObsolete=0,
	@MustAttributes='ap',
	@MayAttributes='meta',
	@IsAbstract=0,
	@IsStructural=1,
	@IsAuxiliary=0

EXEC #Seed_ObjectClasses @Name = 'orgunit',
	@ParentClassName='domain',
	@Description='Root organization unit',
	@IsObsolete=0,
	@MustAttributes='ou',
	@MayAttributes='meta',
	@IsAbstract=0,
	@IsStructural=1,
	@IsAuxiliary=0

EXEC #Seed_ObjectClasses @Name = 'actor',
	@ParentClassName='domain',
	@Description='An actor is a person or system that is using directory to authorized behaivor or to interact directly with directory',
	@IsObsolete=0,
	@MustAttributes='ac',
	@MayAttributes='meta',
	@IsAbstract=0,
	@IsStructural=1,
	@IsAuxiliary=0

EXEC #Seed_ObjectClasses @Name = 'domaingroup',
	@ParentClassName='domain',
	@Description='Named collection of actors or other named collections',
	@IsObsolete=0,
	@MustAttributes='dg',
	@MayAttributes='meta',
	@IsAbstract=0,
	@IsStructural=1,
	@IsAuxiliary=0

ROLLBACK
--COMMIT
DROP PROC IF EXISTS #Seed_ObjectClasses
GO