using Core.Entities;

namespace Core.Test.Entities;

public class MileageRecordTests
{
    private readonly Guid _vehicleId = Guid.NewGuid();

    [Fact]
    public void Create_ValidRecord_ReturnsRecord()
    {
        var record = new MileageRecord(_vehicleId, DateTime.Now.AddDays(-30), 50000);

        Assert.NotEqual(Guid.Empty, record.Id);
        Assert.Equal(_vehicleId, record.VehicleId);
        Assert.Equal(50000, record.Mileage);
        Assert.True(record.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_RecordWithFutureDate_ThrowsArgumentOutOfRangeException()
    {
        var futureDate = DateTime.Now.AddDays(1);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => new MileageRecord(_vehicleId, futureDate, 50000));
    }

    [Fact]
    public void Create_RecordWithZeroMileage_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MileageRecord(_vehicleId, DateTime.Now, 0));
    }

    [Fact]
    public void Create_RecordWithNegativeMileage_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MileageRecord(_vehicleId, DateTime.Now, -1));
    }

    [Fact]
    public void IsValid_ValidRecord_ReturnsTrue()
    {
        var record = new MileageRecord(_vehicleId, DateTime.Now.AddDays(-30), 50000);

        Assert.True(record.IsValid);
    }

    [Fact]
    public void IsValid_FutureDate_ReturnsFalse()
    {
        var record = new MileageRecord
        {
            VehicleId = _vehicleId,
            Date = DateTime.Now.AddDays(1),
            Mileage = 50000
        };

        Assert.False(record.IsValid);
    }

    [Fact]
    public void IsValid_ZeroMileage_ReturnsFalse()
    {
        var record = new MileageRecord
        {
            VehicleId = _vehicleId,
            Date = DateTime.Now,
            Mileage = 0
        };

        Assert.False(record.IsValid);
    }

    [Fact]
    public void DefaultConstructor_CreatesRecordWithGeneratedId()
    {
        var record = new MileageRecord();

        Assert.NotEqual(Guid.Empty, record.Id);
        Assert.True(record.CreatedAt <= DateTime.UtcNow);
    }
}