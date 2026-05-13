namespace Core.Entities;

public class MileageRecord
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime Date { get; set; }
    public int Mileage { get; set; }
    public DateTime CreatedAt { get; set; }

    public MileageRecord()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public MileageRecord(Guid vehicleId, DateTime date, int mileage)
    {
        ValidateDate(date);
        ValidateMileage(mileage);

        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        Date = date;
        Mileage = mileage;
        CreatedAt = DateTime.UtcNow;
    }

    private static void ValidateDate(DateTime date)
    {
        if (date > DateTime.Now)
            throw new ArgumentOutOfRangeException(nameof(date), "Date cannot be in the future");
    }

    private static void ValidateMileage(int mileage)
    {
        if (mileage <= 0)
            throw new ArgumentOutOfRangeException(nameof(mileage), "Mileage must be greater than zero");
    }

    public bool IsValid => Date <= DateTime.Now && Mileage > 0;
}