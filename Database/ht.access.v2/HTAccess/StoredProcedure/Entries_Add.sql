CREATE PROCEDURE [Access].[Entries_Add]
	@DN nvarchar(max),
	@DN_Hash binary(64),
    @EntryId bigint OUTPUT
AS
BEGIN
	INSERT INTO Access.Entries (  
    DN,
    DN_Hash
    
) VALUES (
    
    @DN,
    @DN_Hash
)

SET @EntryId = CAST(@@identity AS bigint)

END
