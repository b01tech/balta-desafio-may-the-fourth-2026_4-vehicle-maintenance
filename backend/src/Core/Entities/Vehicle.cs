namespace Core.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int CurrentMileage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Vehicle()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Vehicle(string brand, string model, int year, int currentMileage)
    {
        ValidateBrand(brand);
        ValidateModel(model);
        ValidateYear(year);
        ValidateMileage(currentMileage);

        Id = Guid.NewGuid();
        Brand = brand;
        Model = model;
        Year = year;
        CurrentMileage = currentMileage;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be empty", nameof(brand));
    }

    private static void ValidateModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty", nameof(model));
    }

    private static void ValidateYear(int year)
    {
        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(year), $"Year must be between 1900 and {DateTime.Now.Year + 1}");
    }

    private static void ValidateMileage(int mileage)
    {
        if (mileage < 0)
            throw new ArgumentOutOfRangeException(nameof(mileage), "Mileage cannot be negative");
    }

    public void UpdateMileage(int newMileage)
    {
        ValidateMileage(newMileage);
        if (newMileage < CurrentMileage)
            throw new ArgumentException("New mileage cannot be less than current mileage", nameof(newMileage));
        
        CurrentMileage = newMileage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string brand, string model, int year)
    {
        ValidateBrand(brand);
        ValidateModel(model);
        ValidateYear(year);
        
        Brand = brand;
        Model = model;
        Year = year;
        UpdatedAt = DateTime.UtcNow;
    }
}