using Core.Entities;

namespace Core.Test.Entities;

public class VehicleTests
{
    [Fact]
    public void Create_ValidVehicle_ReturnsVehicleWithGeneratedId()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2020, 50000);

        Assert.NotEqual(Guid.Empty, vehicle.Id);
        Assert.Equal("Toyota", vehicle.Brand);
        Assert.Equal("Corolla", vehicle.Model);
        Assert.Equal(2020, vehicle.Year);
        Assert.Equal(50000, vehicle.CurrentMileage);
        Assert.True(vehicle.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_VehicleWithInvalidYear_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vehicle("Toyota", "Corolla", 1899, 50000));
    }

    [Fact]
    public void Create_VehicleWithFutureYear_ThrowsArgumentOutOfRangeException()
    {
        var futureYear = DateTime.Now.Year + 2;
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vehicle("Toyota", "Corolla", futureYear, 50000));
    }

    [Fact]
    public void Create_VehicleWithNegativeMileage_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Vehicle("Toyota", "Corolla", 2020, -1));
    }

    [Fact]
    public void Create_VehicleWithEmptyBrand_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Vehicle("", "Corolla", 2020, 50000));
    }

    [Fact]
    public void Create_VehicleWithEmptyModel_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Vehicle("Toyota", "", 2020, 50000));
    }

    [Fact]
    public void UpdateMileage_ValidMileage_UpdatesMileage()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2020, 50000);
        
        vehicle.UpdateMileage(55000);

        Assert.Equal(55000, vehicle.CurrentMileage);
        Assert.True(vehicle.UpdatedAt > vehicle.CreatedAt);
    }

    [Fact]
    public void UpdateMileage_LowerMileage_ThrowsArgumentException()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2020, 50000);

        Assert.Throws<ArgumentException>(() => vehicle.UpdateMileage(40000));
    }

    [Fact]
    public void UpdateMileage_NegativeMileage_ThrowsArgumentOutOfRangeException()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2020, 50000);

        Assert.Throws<ArgumentOutOfRangeException>(() => vehicle.UpdateMileage(-1));
    }

    [Fact]
    public void Update_ValidData_UpdatesAllProperties()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2020, 50000);
        
        vehicle.Update("Honda", "Civic", 2021);

        Assert.Equal("Honda", vehicle.Brand);
        Assert.Equal("Civic", vehicle.Model);
        Assert.Equal(2021, vehicle.Year);
    }

    [Fact]
    public void DefaultConstructor_CreatesVehicleWithGeneratedId()
    {
        var vehicle = new Vehicle();

        Assert.NotEqual(Guid.Empty, vehicle.Id);
        Assert.True(vehicle.CreatedAt <= DateTime.UtcNow);
    }
}