USE [ht.access.v2]
GO
DROP PROC IF EXISTS #Seed_SystemAttributes
GO
CREATE PROC #Seed_SystemAttributes (
	@TypeCode varchar(20)='any',
	@Name varchar(60),
	@Alias varchar(60)=null,
	@IsObsolete bit=0,
	@Description nvarchar(1000) = null,
	@AllowMultipleValues bit=0,
	@AllowUserModification bit=1
) AS
BEGIN
	DECLARE @AttributeId INT
	DECLARE @ValueId INT
	SELECT TOP(1) @AttributeId = AttributeId FROM Access.Attributes WITH (NOLOCK) WHERE [Name] = @Name
	SELECT TOP(1) @ValueId = AttributeValueTypeId FROM Access.AttributeValueTypes WITH (NOLOCK) WHERE [Type] = @TypeCode
	SET @ValueId = Coalesce(@ValueId,0)
	SET @Name = lower(@Name)

	IF @AttributeId IS NULL
	BEGIN
		INSERT INTO Access.Attributes (
			AttributeTypeId,
			IsObsolete,
			[Name],
			[Description],
			AllowMultipleValues,
			AllowUserModification,
			Alias
		) VALUES (
			@ValueId,
			@IsObsolete,
			@Name,
			@Description,
			@AllowMultipleValues,
			@AllowUserModification,
			@Alias
		)
	END
	ELSE
	BEGIN
		UPDATE TOP(1)
			Access.Attributes
		SET
			AttributeTypeId = @ValueId,
			IsObsolete = @IsObsolete,
			[Description] = @Description,
			AllowMultipleValues = @AllowMultipleValues,
			AllowUserModification = @AllowUserModification,
			Alias = @Alias

		WHERE
			AttributeId = @AttributeId 	
	END
END
GO

BEGIN TRANSACTION
EXEC #Seed_SystemAttributes @Name='do',@Description='Root domain of a directory tree. Similar to multi-tenant capability (e.g. dev-env, prod-env)',@TypeCode='string',@AllowMultipleValues=0

--EXEC #Seed_SystemAttributes @Name='Code',@Description='System identifier of this attribute.  Must be unique in directory.',@TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='Description', @Description='Short human readable description that describes this object', @TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='UID', @Description='Unique identifier suitable for displaying to insecure clients',@TypeCode='string',@AllowUserModification=0
--EXEC #Seed_SystemAttributes @Name='do',@Alias='domain', @Description='Root container for all other directory objects', @TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='lastUpdate',@Alias='domain', @Description='Last time this object was modified', @TypeCode='date',@AllowMultipleValues=0,@AllowUserModification=0
--EXEC #Seed_SystemAttributes @Name='objectClass',@Alias=null, @Description='Name of object class', @TypeCode='string',@AllowMultipleValues=1,@AllowUserModification=1
--EXEC #Seed_SystemAttributes @Name='ac',@Alias='actor', @Description='Identifier of a particular actor that is demanding access to objects', @TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='ap',@Alias='application', @Description='Identifier for this application across entire directory', @TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='o',@Alias='Org', @Description='Root org unit identifier in this directory', @TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='ou',@Alias='OrgUnit', @Description='Sub division within an organization', @TypeCode='string',@AllowMultipleValues=0
--EXEC #Seed_SystemAttributes @Name='dg',@Alias='DomainGroup', @Description='Group of actors or objects containing actors not necessarily bound to other containers', @TypeCode='string',@AllowMultipleValues=0

ROLLBACK
--COMMIT
GO
DROP PROC IF EXISTS #Seed_SystemAttributes
go 