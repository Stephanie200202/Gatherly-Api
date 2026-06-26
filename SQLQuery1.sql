-- Make the UserId column allow NULL values for guests
ALTER TABLE Registrations ALTER COLUMN UserId UNIQUEIDENTIFIER NULL;

-- Add the tracking columns for guest info
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Registrations') AND name = 'GuestEmail')
BEGIN
    ALTER TABLE Registrations ADD GuestEmail NVARCHAR(256) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Registrations') AND name = 'GuestName')
BEGIN
    ALTER TABLE Registrations ADD GuestName NVARCHAR(256) NULL;
END
