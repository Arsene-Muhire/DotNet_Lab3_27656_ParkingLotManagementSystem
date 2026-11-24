# Parking Lot Management System (PLMS)

**Student:** MURIHIRA MUHIRE Arsene
**ID:** 27656
**Course:** .NET, Lab 3, Adventist University of Central Africa (AUCA)

A Windows desktop application that replaces the paper book used at a parking lot gate. The attendant registers each vehicle that enters and assigns it a slot. When the vehicle leaves, the system calculates the fee automatically, records the payment, and produces reports.

Built with **C# WinForms**, following the **MVC** architecture, and stores its data in **MySQL (XAMPP)** through **ADO.NET**.

## Features

| Area | What it does |
|---|---|
| **Login and users** | Login with hashed passwords (PBKDF2). Two roles: Administrator and Attendant. The Administrator can add, edit, search, activate, deactivate and delete users. Every user can change their own password. |
| **Parking slots** | Add, edit and delete slots with a number, a vehicle type (Motorcycle, Car, Bus/Truck) and a zone. Slot statuses are Available, Occupied and Out of Service. Slots can be filtered by status, type and zone, or searched by number. |
| **Rates** | Price per hour for each vehicle type. A new rate applies only to vehicles that check in after the change. |
| **Check-in** | Plate, vehicle type, owner and phone. Only Available slots of the matching type are shown. Generates a ticket number and shows the entry ticket. |
| **Check-out** | Find a vehicle by ticket or plate. Calculates hours (rounded up, minimum 1) times the rate. Records Cash or Mobile Money, shows the change, and prints a receipt on screen. |
| **Vehicle records** | All records, with search by plate or ticket and filters by date, status and vehicle type. Completed records cannot be deleted. |
| **Dashboard** | Total, Available, Occupied and Out of Service slots, vehicles currently parked, and money collected today. |
| **Reports** | Vehicles currently parked, daily income, income by period (by vehicle type and by payment method), and slot occupancy. |

## Business rules enforced

- A slot holds only one vehicle at a time, and a vehicle cannot be checked in twice. The database enforces both with unique indexes, not only the code.
- A vehicle can only use an Available slot of its own type.
- Parking hours are exit time minus entry time, rounded up to the next full hour, with a minimum of one hour.
- The fee is hours times the rate per hour. Check-out is refused if the amount received is less than the fee.
- A slot that is Occupied or has parking records cannot be deleted.
- Only the Administrator can manage users, slots and rates.

## Project structure (MVC)

```
ParkingLotManagementSystem/
├── Models/            Entities (User, ParkingSlot, Rate, Vehicle, ParkingRecord), fee rules, session
│   └── Data/          ADO.NET data access (MySqlConnection, MySqlCommand, parameterised queries)
├── Controllers/       Business logic and validation, one controller per feature
├── Views/             WinForms forms (Form.cs = logic, Form.Designer.cs = layout, editable in the designer)
├── Helpers/           Password hashing
└── App.config         Database connection string
Database/
└── PLMS_Database.sql  Script that creates the database and its five tables
```

## Requirements

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download), or Visual Studio 2026 with the *.NET desktop development* workload
- [XAMPP](https://www.apachefriends.org/) (MySQL / MariaDB)

## How to run

1. Open the **XAMPP Control Panel** and click **Start** next to **MySQL**.
2. Open `ParkingLotManagementSystem.slnx` in Visual Studio and press **F5**, or run this from the project folder:
   ```
   dotnet run --project ParkingLotManagementSystem
   ```
3. On first start, the application creates the `plms_db` database and its tables automatically. It also adds default rates, 18 sample slots and two user accounts.

You can also create the database by hand: in phpMyAdmin, open the **Import** tab and import `Database/PLMS_Database.sql`.

### Default accounts

| Username | Password | Role |
|---|---|---|
| `admin` | `Muhire123` | Administrator |
| `attendant` | `Muhire123` | Attendant |

Change these passwords after the first login.

### Connection settings

The connection string is in `ParkingLotManagementSystem/App.config`:

```
Server=localhost;Port=3306;Database=plms_db;Uid=root;Pwd=;
```

If your MySQL `root` user has a password, put it after `Pwd=`.

## Database tables

| Table | Main columns |
|---|---|
| Users | UserID, FullName, Username (unique), PasswordHash, Role, IsActive |
| ParkingSlots | SlotID, SlotNo (unique), VehicleType, Zone, Status |
| Rates | RateID, VehicleType (unique), PricePerHour |
| Vehicles | VehicleID, PlateNo (unique), VehicleType, OwnerName, Phone |
| ParkingRecords | RecordID, TicketNo (unique), VehicleID, SlotID, UserID, EntryTime, ExitTime, Hours, RatePerHour, Fee, AmountPaid, PaymentMethod, Status |
