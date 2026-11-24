using MySql.Data.MySqlClient;

namespace ParkingLotManagementSystem.Models.Data;

public class ParkingRecordRepository
{
    private const string SelectColumns =
        @"SELECT r.RecordID, r.TicketNo, r.VehicleID, r.SlotID, r.UserID, r.EntryTime, r.ExitTime, r.Hours,
                 r.RatePerHour, r.Fee, r.AmountPaid, r.PaymentMethod, r.Status,
                 v.PlateNo, v.VehicleType, v.OwnerName, v.Phone,
                 s.SlotNo, s.Zone, u.FullName AS AttendantName
          FROM ParkingRecords r
          INNER JOIN Vehicles v ON v.VehicleID = r.VehicleID
          INNER JOIN ParkingSlots s ON s.SlotID = r.SlotID
          INNER JOIN Users u ON u.UserID = r.UserID";

    private readonly VehicleRepository _vehicles = new();

    public ParkingRecord? GetById(int recordId)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection, $"{SelectColumns} WHERE r.RecordID = @id");
        command.AddParam("@id", recordId);
        var records = ReadList(command);
        return records.Count > 0 ? records[0] : null;
    }

    public ParkingRecord? FindParked(string ticketNo, string plateNo)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $"{SelectColumns} WHERE r.Status = @status AND (r.TicketNo = @ticket OR v.PlateNo = @plate)");
        command.AddParam("@status", RecordStatuses.Parked)
               .AddParam("@ticket", ticketNo)
               .AddParam("@plate", plateNo);
        var records = ReadList(command);
        return records.Count > 0 ? records[0] : null;
    }

    public bool IsVehicleParked(string plateNo)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            @"SELECT COUNT(*) FROM ParkingRecords r
              INNER JOIN Vehicles v ON v.VehicleID = r.VehicleID
              WHERE v.PlateNo = @plate AND r.Status = @status");
        command.AddParam("@plate", plateNo).AddParam("@status", RecordStatuses.Parked);
        return command.ExecuteCount() > 0;
    }

    public List<ParkingRecord> Search(string? keyword, DateTime? entryDate, string? status, string? vehicleType)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $@"{SelectColumns}
               WHERE (@keyword IS NULL OR v.PlateNo LIKE @keyword OR r.TicketNo LIKE @keyword)
                 AND (@dateFrom IS NULL OR (r.EntryTime >= @dateFrom AND r.EntryTime < @dateTo))
                 AND (@status IS NULL OR r.Status = @status)
                 AND (@type IS NULL OR v.VehicleType = @type)
               ORDER BY r.EntryTime DESC");
        command.AddParam("@keyword", string.IsNullOrWhiteSpace(keyword) ? null : SqlExtensions.ToLikePattern(keyword.Trim()))
               .AddParam("@dateFrom", entryDate?.Date)
               .AddParam("@dateTo", entryDate?.Date.AddDays(1))
               .AddParam("@status", status)
               .AddParam("@type", vehicleType);
        return ReadList(command);
    }

    public List<ParkingRecord> GetCurrentlyParked()
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $"{SelectColumns} WHERE r.Status = @status ORDER BY r.EntryTime");
        command.AddParam("@status", RecordStatuses.Parked);
        return ReadList(command);
    }

    public List<ParkingRecord> GetCompletedBetween(DateTime from, DateTime toExclusive)
    {
        using var connection = Database.OpenConnection();
        using var command = Database.CreateCommand(connection,
            $@"{SelectColumns}
               WHERE r.Status = @status AND r.ExitTime >= @from AND r.ExitTime < @to
               ORDER BY r.ExitTime");
        command.AddParam("@status", RecordStatuses.Completed).AddParam("@from", from).AddParam("@to", toExclusive);
        return ReadList(command);
    }

    public ParkingRecord CheckIn(Vehicle vehicle, int slotId, int userId, decimal ratePerHour, DateTime entryTime)
    {
        int recordId;
        using (var connection = Database.OpenConnection())
        using (var transaction = connection.BeginTransaction())
        {
            var existing = _vehicles.GetByPlateForUpdate(connection, transaction, vehicle.PlateNo);
            if (existing == null)
            {
                vehicle.VehicleID = _vehicles.Insert(connection, transaction, vehicle);
            }
            else
            {
                vehicle.VehicleID = existing.VehicleID;
                using var parkedCheck = Database.CreateCommand(connection,
                    "SELECT COUNT(*) FROM ParkingRecords WHERE VehicleID = @id AND Status = @status", transaction);
                parkedCheck.AddParam("@id", vehicle.VehicleID).AddParam("@status", RecordStatuses.Parked);
                if (parkedCheck.ExecuteCount() > 0)
                {
                    throw new BusinessRuleException($"Vehicle {vehicle.PlateNo} is already parked in the lot.");
                }

                _vehicles.Update(connection, transaction, vehicle);
            }

            using (var occupy = Database.CreateCommand(connection,
                       @"UPDATE ParkingSlots SET Status = @occupied
                         WHERE SlotID = @slotId AND Status = @available AND VehicleType = @type", transaction))
            {
                occupy.AddParam("@occupied", SlotStatuses.Occupied)
                      .AddParam("@slotId", slotId)
                      .AddParam("@available", SlotStatuses.Available)
                      .AddParam("@type", vehicle.VehicleType);
                if (occupy.ExecuteNonQuery() == 0)
                {
                    throw new BusinessRuleException("The selected slot is no longer available for this vehicle type. Please choose another slot.");
                }
            }

            using (var insert = Database.CreateCommand(connection,
                       @"INSERT INTO ParkingRecords (TicketNo, VehicleID, SlotID, UserID, EntryTime, RatePerHour, Status)
                         VALUES (@ticket, @vehicleId, @slotId, @userId, @entry, @rate, @status)", transaction))
            {
                insert.AddParam("@ticket", "TMP" + Guid.NewGuid().ToString("N")[..24])
                      .AddParam("@vehicleId", vehicle.VehicleID)
                      .AddParam("@slotId", slotId)
                      .AddParam("@userId", userId)
                      .AddParam("@entry", entryTime)
                      .AddParam("@rate", ratePerHour)
                      .AddParam("@status", RecordStatuses.Parked);
                recordId = insert.ExecuteInsert();
            }

            var ticketNo = $"TKT{entryTime:yyyyMMdd}-{recordId:D5}";
            using (var ticket = Database.CreateCommand(connection,
                       "UPDATE ParkingRecords SET TicketNo = @ticket WHERE RecordID = @id", transaction))
            {
                ticket.AddParam("@ticket", ticketNo).AddParam("@id", recordId);
                ticket.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        return GetById(recordId)!;
    }

    public void CheckOut(int recordId, int slotId, DateTime exitTime, int hours, decimal fee, decimal amountPaid, string paymentMethod)
    {
        using var connection = Database.OpenConnection();
        using var transaction = connection.BeginTransaction();

        using (var complete = Database.CreateCommand(connection,
                   @"UPDATE ParkingRecords
                     SET ExitTime = @exit, Hours = @hours, Fee = @fee, AmountPaid = @paid,
                         PaymentMethod = @method, Status = @completed
                     WHERE RecordID = @id AND Status = @parked", transaction))
        {
            complete.AddParam("@exit", exitTime)
                    .AddParam("@hours", hours)
                    .AddParam("@fee", fee)
                    .AddParam("@paid", amountPaid)
                    .AddParam("@method", paymentMethod)
                    .AddParam("@completed", RecordStatuses.Completed)
                    .AddParam("@id", recordId)
                    .AddParam("@parked", RecordStatuses.Parked);
            if (complete.ExecuteNonQuery() == 0)
            {
                throw new BusinessRuleException("This vehicle has already been checked out.");
            }
        }

        using (var release = Database.CreateCommand(connection,
                   "UPDATE ParkingSlots SET Status = @available WHERE SlotID = @slotId", transaction))
        {
            release.AddParam("@available", SlotStatuses.Available).AddParam("@slotId", slotId);
            release.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public void DeleteParked(int recordId, int slotId)
    {
        using var connection = Database.OpenConnection();
        using var transaction = connection.BeginTransaction();

        using (var delete = Database.CreateCommand(connection,
                   "DELETE FROM ParkingRecords WHERE RecordID = @id AND Status = @parked", transaction))
        {
            delete.AddParam("@id", recordId).AddParam("@parked", RecordStatuses.Parked);
            if (delete.ExecuteNonQuery() == 0)
            {
                throw new BusinessRuleException("A completed record cannot be deleted.");
            }
        }

        using (var release = Database.CreateCommand(connection,
                   "UPDATE ParkingSlots SET Status = @available WHERE SlotID = @slotId AND Status = @occupied", transaction))
        {
            release.AddParam("@available", SlotStatuses.Available)
                   .AddParam("@slotId", slotId)
                   .AddParam("@occupied", SlotStatuses.Occupied);
            release.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private static List<ParkingRecord> ReadList(MySqlCommand command)
    {
        var records = new List<ParkingRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            records.Add(new ParkingRecord
            {
                RecordID = reader.GetInt("RecordID"),
                TicketNo = reader.GetText("TicketNo"),
                VehicleID = reader.GetInt("VehicleID"),
                SlotID = reader.GetInt("SlotID"),
                UserID = reader.GetInt("UserID"),
                EntryTime = reader.GetDate("EntryTime"),
                ExitTime = reader.GetNullableDate("ExitTime"),
                Hours = reader.GetNullableInt("Hours"),
                RatePerHour = reader.GetMoney("RatePerHour"),
                Fee = reader.GetNullableMoney("Fee"),
                AmountPaid = reader.GetNullableMoney("AmountPaid"),
                PaymentMethod = reader.GetNullableText("PaymentMethod"),
                Status = reader.GetText("Status"),
                PlateNo = reader.GetText("PlateNo"),
                VehicleType = reader.GetText("VehicleType"),
                OwnerName = reader.GetText("OwnerName"),
                Phone = reader.GetText("Phone"),
                SlotNo = reader.GetText("SlotNo"),
                Zone = reader.GetText("Zone"),
                AttendantName = reader.GetText("AttendantName")
            });
        }

        return records;
    }
}
