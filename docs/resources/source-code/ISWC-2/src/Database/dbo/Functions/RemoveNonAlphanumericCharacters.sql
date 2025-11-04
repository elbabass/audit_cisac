CREATE Function [RemoveNonAlphanumericCharacters](@strText VARCHAR(1000))
RETURNS VARCHAR(1000)
AS
BEGIN
    WHILE PATINDEX('%[^a-zA-Z0-9]%', @strText) > 0
    BEGIN
        SET @strText = STUFF(@strText, PATINDEX('%[^a-zA-Z0-9]%', @strText), 1, '')
    END
    RETURN @strText
END