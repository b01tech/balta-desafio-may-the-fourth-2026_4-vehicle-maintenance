namespace Core.Entities;

public class MaintenancePart
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal EstimatedPrice { get; set; }
    public string? PartNumber { get; set; }

    public MaintenancePart()
    {
    }

    public MaintenancePart(string name, int quantity, decimal estimatedPrice, string? partNumber = null)
    {
        ValidateName(name);
        ValidateQuantity(quantity);
        ValidatePrice(estimatedPrice);

        Name = name;
        Quantity = quantity;
        EstimatedPrice = estimatedPrice;
        PartNumber = partNumber;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero");
    }

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
    }

    public decimal TotalPrice => EstimatedPrice * Quantity;
}