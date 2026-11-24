CREATE DATABASE IF NOT EXISTS plms_db CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

USE plms_db;

CREATE TABLE IF NOT EXISTS Users (
    UserID INT NOT NULL AUTO_INCREMENT,
    FullName VARCHAR(100) NOT NULL,
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(256) NOT NULL,
    Role VARCHAR(20) NOT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_Users PRIMARY KEY (UserID),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Administrator', 'Attendant'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ParkingSlots (
    SlotID INT NOT NULL AUTO_INCREMENT,
    SlotNo VARCHAR(20) NOT NULL,
    VehicleType VARCHAR(20) NOT NULL,
    Zone VARCHAR(50) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Available',
    CONSTRAINT PK_ParkingSlots PRIMARY KEY (SlotID),
    CONSTRAINT UQ_ParkingSlots_SlotNo UNIQUE (SlotNo),
    CONSTRAINT CK_ParkingSlots_VehicleType CHECK (VehicleType IN ('Motorcycle', 'Car', 'Bus/Truck')),
    CONSTRAINT CK_ParkingSlots_Status CHECK (Status IN ('Available', 'Occupied', 'Out of Service'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Rates (
    RateID INT NOT NULL AUTO_INCREMENT,
    VehicleType VARCHAR(20) NOT NULL,
    PricePerHour DECIMAL(12,2) NOT NULL,
    CONSTRAINT PK_Rates PRIMARY KEY (RateID),
    CONSTRAINT UQ_Rates_VehicleType UNIQUE (VehicleType),
    CONSTRAINT CK_Rates_VehicleType CHECK (VehicleType IN ('Motorcycle', 'Car', 'Bus/Truck')),
    CONSTRAINT CK_Rates_Price CHECK (PricePerHour > 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Vehicles (
    VehicleID INT NOT NULL AUTO_INCREMENT,
    PlateNo VARCHAR(20) NOT NULL,
    VehicleType VARCHAR(20) NOT NULL,
    OwnerName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    CONSTRAINT PK_Vehicles PRIMARY KEY (VehicleID),
    CONSTRAINT UQ_Vehicles_PlateNo UNIQUE (PlateNo),
    CONSTRAINT CK_Vehicles_VehicleType CHECK (VehicleType IN ('Motorcycle', 'Car', 'Bus/Truck'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ParkingRecords (
    RecordID INT NOT NULL AUTO_INCREMENT,
    TicketNo VARCHAR(30) NOT NULL,
    VehicleID INT NOT NULL,
    SlotID INT NOT NULL,
    UserID INT NOT NULL,
    EntryTime DATETIME NOT NULL,
    ExitTime DATETIME NULL,
    Hours INT NULL,
    RatePerHour DECIMAL(12,2) NOT NULL,
    Fee DECIMAL(12,2) NULL,
    AmountPaid DECIMAL(12,2) NULL,
    PaymentMethod VARCHAR(20) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Parked',
    ParkedSlotID INT GENERATED ALWAYS AS (CASE WHEN Status = 'Parked' THEN SlotID ELSE NULL END) STORED,
    ParkedVehicleID INT GENERATED ALWAYS AS (CASE WHEN Status = 'Parked' THEN VehicleID ELSE NULL END) STORED,
    CONSTRAINT PK_ParkingRecords PRIMARY KEY (RecordID),
    CONSTRAINT UQ_ParkingRecords_TicketNo UNIQUE (TicketNo),
    CONSTRAINT UX_ParkingRecords_ParkedSlot UNIQUE (ParkedSlotID),
    CONSTRAINT UX_ParkingRecords_ParkedVehicle UNIQUE (ParkedVehicleID),
    CONSTRAINT FK_ParkingRecords_Vehicles FOREIGN KEY (VehicleID) REFERENCES Vehicles (VehicleID),
    CONSTRAINT FK_ParkingRecords_ParkingSlots FOREIGN KEY (SlotID) REFERENCES ParkingSlots (SlotID),
    CONSTRAINT FK_ParkingRecords_Users FOREIGN KEY (UserID) REFERENCES Users (UserID),
    CONSTRAINT CK_ParkingRecords_PaymentMethod CHECK (PaymentMethod IS NULL OR PaymentMethod IN ('Cash', 'Mobile Money')),
    CONSTRAINT CK_ParkingRecords_Status CHECK (Status IN ('Parked', 'Completed')),
    INDEX IX_ParkingRecords_EntryTime (EntryTime),
    INDEX IX_ParkingRecords_ExitTime (ExitTime, Status),
    INDEX IX_ParkingRecords_Status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Rates (VehicleType, PricePerHour) VALUES
    ('Motorcycle', 200),
    ('Car', 500),
    ('Bus/Truck', 1000);
