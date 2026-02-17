CREATE DATABASE HotelReservationDb;
GO
USE HotelReservationDb;
GO

CREATE TABLE Roles (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(120) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE HotelClasses (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    BasePricePerNight DECIMAL(18,2) NOT NULL,
    ImageUrl NVARCHAR(300),
    AvailableServices NVARCHAR(400),
    SpecialOffers NVARCHAR(400)
);

CREATE TABLE Rooms (
    Id INT IDENTITY PRIMARY KEY,
    RoomNumber NVARCHAR(20) NOT NULL UNIQUE,
    HotelClassId INT NOT NULL,
    MaxGuests INT NOT NULL,
    PricePerNight DECIMAL(18,2) NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (HotelClassId) REFERENCES HotelClasses(Id)
);

CREATE TABLE Services (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Description NVARCHAR(300)
);

CREATE TABLE AddOns (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Reservations (
    Id INT IDENTITY PRIMARY KEY,
    BookingReference NVARCHAR(20) NOT NULL UNIQUE,
    UserId INT NOT NULL,
    RoomId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    NumberOfGuests INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RoomId) REFERENCES Rooms(Id),
    CONSTRAINT CK_Reservation_Dates CHECK (CheckOutDate > CheckInDate)
);

CREATE TABLE ReservationAddOns (
    ReservationId INT NOT NULL,
    AddOnId INT NOT NULL,
    PRIMARY KEY (ReservationId, AddOnId),
    FOREIGN KEY (ReservationId) REFERENCES Reservations(Id),
    FOREIGN KEY (AddOnId) REFERENCES AddOns(Id)
);

CREATE TABLE Payments (
    Id INT IDENTITY PRIMARY KEY,
    ReservationId INT NOT NULL UNIQUE,
    Method NVARCHAR(20) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaidAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (ReservationId) REFERENCES Reservations(Id)
);

CREATE TABLE Receipts (
    Id INT IDENTITY PRIMARY KEY,
    ReservationId INT NOT NULL UNIQUE,
    ReceiptNumber NVARCHAR(30) NOT NULL,
    EmailSentTo NVARCHAR(120),
    IssuedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (ReservationId) REFERENCES Reservations(Id)
);
GO

-- Prevent double booking rule using trigger
CREATE TRIGGER trg_PreventDoubleBooking ON Reservations
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN Reservations r ON i.RoomId = r.RoomId
        WHERE i.CheckInDate < r.CheckOutDate
          AND i.CheckOutDate > r.CheckInDate
          AND r.Status <> 'Cancelled'
    )
    BEGIN
        RAISERROR ('Room is already reserved for the selected date range.', 16, 1);
        RETURN;
    END

    INSERT INTO Reservations (BookingReference, UserId, RoomId, CheckInDate, CheckOutDate, NumberOfGuests, Status, TotalPrice, CreatedAt)
    SELECT BookingReference, UserId, RoomId, CheckInDate, CheckOutDate, NumberOfGuests, Status, TotalPrice, CreatedAt
    FROM inserted;
END;
GO
