USE [ht.access.v2]
GO
DROP PROC IF EXISTS #Seed_AttributeTypes
GO
SET IDENTITY_INSERT [Access].[AttributeValueTypes]  ON
GO
CREATE PROC #Seed_AttributeTypes (
	@Id int,
	@TypeName varchar(20)
) AS
BEGIN
	UPDATE TOP(1)
		Access.AttributeValueTypes
	SET
		[Type] = @TypeName
	WHERE
		AttributeValueTypeId = @Id
	IF @@ROWCOUNT < 1
	BEGIN
		INSERT INTO	[Access].[AttributeValueTypes] (
			AttributeValueTypeId,
			[Type]
		) VALUES (
			@Id,
			@TypeName
		)
	END

END
GO
BEGIN TRANSACTION
EXEC #Seed_AttributeTypes @Id=0, @TypeName="unknown"
EXEC #Seed_AttributeTypes @Id=1, @TypeName="string"
EXEC #Seed_AttributeTypes @Id=2, @TypeName="number"
EXEC #Seed_AttributeTypes @Id=3, @TypeName="date"
EXEC #Seed_AttributeTypes @Id=4, @TypeName="boolean"
EXEC #Seed_AttributeTypes @Id=5, @TypeName="link"

SET IDENTITY_INSERT [Access].[AttributeValueTypes]  OFF
ROLLBACK
--COMMIT
GO
DROP PROC IF EXISTS #Seed_AttributeTypes