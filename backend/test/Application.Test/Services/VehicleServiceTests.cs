using Application.DTOs;
using Application.Services;
using Core.Interfaces;
using Moq;

namespace Application.Test.Services;

public class VehicleServiceTests
{
    private readonly Mock<IMileageDataReader> _mockMileageReader;
    private readonly VehicleService _service;

    public VehicleServiceTests()
    {
        _mockMileageReader = new Mock<IMileageDataReader>();
        _service = new VehicleService(_mockMileageReader.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedVehicle()
    {
        var dto = new CreateVehicleDto("Toyota", "Corolla", 2020, 50000);

        var result = await _service.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Toyota", result.Brand);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(2020, result.Year);
        Assert.Equal(50000, result.CurrentMileage);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsVehicle()
    {
        var dto = new CreateVehicleDto("Toyota", "Corolla", 2020, 50000);
        var created = await _service.CreateAsync(dto);

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllVehicles()
    {
        await _service.CreateAsync(new CreateVehicleDto("Toyota", "Corolla", 2020, 50000));
        await _service.CreateAsync(new CreateVehicleDto("Honda", "Civic", 2021, 30000));

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ValidData_UpdatesVehicle()
    {
        var created = await _service.CreateAsync(new CreateVehicleDto("Toyota", "Corolla", 2020, 50000));

        var result = await _service.UpdateAsync(created.Id, new UpdateVehicleDto("Toyota", "Corolla", 2021, 55000));

        Assert.Equal(2021, result.Year);
        Assert.Equal(55000, result.CurrentMileage);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(Guid.NewGuid(), new UpdateVehicleDto("Toyota", "Corolla", 2020, 50000)));
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        var created = await _service.CreateAsync(new CreateVehicleDto("Toyota", "Corolla", 2020, 50000));

        var result = await _service.DeleteAsync(created.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task ImportMileageFromCsvAsync_ValidCsv_UpdatesMileage()
    {
        var created = await _service.CreateAsync(new CreateVehicleDto("Toyota", "Corolla", 2020, 50000));
        var csv = "data,quilometragem\n2024-01-01,60000";
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        _mockMileageReader
            .Setup(r => r.ReadFromCsvAsync(It.IsAny<Stream>(), It.IsAny<Guid>()))
            .ReturnsAsync(new List<Core.Entities.MileageRecord>
            {
                new(created.Id, new DateTime(2024, 1, 1), 60000)
            });

        await _service.ImportMileageFromCsvAsync(created.Id, stream);

        var result = await _service.GetByIdAsync(created.Id);
        Assert.Equal(60000, result!.CurrentMileage);
    }

    [Fact]
    public async Task ImportMileageFromCsvAsync_VehicleNotFound_ThrowsKeyNotFoundException()
    {
        var csv = "data,quilometragem\n2024-01-01,60000";
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.ImportMileageFromCsvAsync(Guid.NewGuid(), stream));
    }

    [Fact]
    public async Task GetMileageRecordsAsync_ReturnsEmptyList()
    {
        var created = await _service.CreateAsync(new CreateVehicleDto("Toyota", "Corolla", 2020, 50000));

        var result = await _service.GetMileageRecordsAsync(created.Id);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMileageRecordsAsync_VehicleNotFound_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetMileageRecordsAsync(Guid.NewGuid()));
    }
}