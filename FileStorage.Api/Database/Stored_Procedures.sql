USE FileStorageDB;
GO
IF OBJECT_ID('SP_CheckAndUpdateQuota', 'P') IS NOT NULL
    DROP PROCEDURE SP_CheckAndUpdateQuota;
GO

CREATE PROCEDURE SP_CheckAndUpdateQuota
    @UserId INT,
    @DeltaBytes BIGINT,
    @Success BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET UsedStorageBytes = UsedStorageBytes + @DeltaBytes,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @UserId 
      AND IsActive = 1
      AND (UsedStorageBytes + @DeltaBytes) <= MaxStorageBytes;

    IF @@ROWCOUNT > 0
        SET @Success = 1;
    ELSE
        SET @Success = 0;
END
GO

IF OBJECT_ID('SP_GetUserWithQuota', 'P') IS NOT NULL
    DROP PROCEDURE SP_GetUserWithQuota;
GO

CREATE PROCEDURE SP_GetUserWithQuota
    @UserId INT
AS
BEGIN
    SELECT Id, Username, Email, Role, 
           MaxStorageBytes, UsedStorageBytes,
           IsActive, CreatedAt
    FROM Users 
    WHERE Id = @UserId AND IsActive = 1;
END
GO

PRINT '✅ Stored Procedures created successfully!';