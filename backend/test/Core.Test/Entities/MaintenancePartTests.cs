using Core.Entities;

namespace Core.Test.Entities;

public class MaintenancePartTests
{
    [Fact]
    public void Create_ValidPart_ReturnsPart()
    {
        var part = new MaintenancePart("Filtro de Óleo", 1, 45.00m, "FO-001");

        Assert.Equal("Filtro de Óleo", part.Name);
        Assert.Equal(1, part.Quantity);
        Assert.Equal(45.00m, part.EstimatedPrice);
        Assert.Equal("FO-001", part.PartNumber);
    }

    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MaintenancePart("", 1, 45.00m));
    }

    [Fact]
    public void Create_WhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MaintenancePart("   ", 1, 45.00m));
    }

    [Fact]
    public void Create_ZeroQuantity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MaintenancePart("Filtro", 0, 45.00m));
    }

    [Fact]
    public void Create_NegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MaintenancePart("Filtro", -1, 45.00m));
    }

    [Fact]
    public void Create_NegativePrice_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MaintenancePart("Filtro", 1, -10.00m));
    }

    [Fact]
    public void TotalPrice_CalculatesCorrectly()
    {
        var part = new MaintenancePart("Óleo", 4, 50.00m);

        Assert.Equal(200.00m, part.TotalPrice);
    }

    [Fact]
    public void Create_WithoutPartNumber_SetsNull()
    {
        var part = new MaintenancePart("Filtro", 1, 45.00m);

        Assert.Null(part.PartNumber);
    }

    [Fact]
    public void Create_ZeroPrice_Succeeds()
    {
        var part = new MaintenancePart("Bujão", 1, 0m);

        Assert.Equal(0m, part.EstimatedPrice);
    }

    [Fact]
    public void DefaultConstructor_CreatesEmptyPart()
    {
        var part = new MaintenancePart();

        Assert.Equal(string.Empty, part.Name);
        Assert.Equal(0, part.Quantity);
        Assert.Equal(0m, part.EstimatedPrice);
    }
}