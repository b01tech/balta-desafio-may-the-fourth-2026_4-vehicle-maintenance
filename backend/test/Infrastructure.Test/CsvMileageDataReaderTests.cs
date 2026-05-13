using Core.Interfaces;
using Infrastructure.Readers;

namespace Infrastructure.Test;

public class CsvMileageDataReaderTests
{
    private readonly CsvMileageDataReader _reader = new();
    private readonly Guid _vehicleId = Guid.NewGuid();

    [Fact]
    public async Task ReadFromCsvAsync_ValidCsv_ReturnsOrderedRecords()
    {
        var csv = "data,quilometragem\n2024-01-01,50000\n2024-06-01,55000\n2024-12-01,60000";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Equal(3, records.Count);
        Assert.Equal(50000, records[0].Mileage);
        Assert.Equal(55000, records[1].Mileage);
        Assert.Equal(60000, records[2].Mileage);
    }

    [Fact]
    public async Task ReadFromCsvAsync_EmptyCsv_ReturnsEmptyList()
    {
        var csv = "";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = await _reader.ReadFromCsvAsync(stream, _vehicleId);

        Assert.Empty(records);
    }

    [Fact]
    public async Task ReadFromCsvAsync_CsvWithEmptyLines_SkipsEmptyLines()
    {
        var csv = "data,quilometragem\n2024-01-01,50000\n\n2024-06-01,55000\n";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Equal(2, records.Count);
    }

    [Fact]
    public async Task ReadFromCsvAsync_InvalidDate_SkipsInvalidRow()
    {
        var csv = "data,quilometragem\n2024-01-01,50000\ninvalid,55000\n2024-12-01,60000";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Equal(2, records.Count);
    }

    [Fact]
    public async Task ReadFromCsvAsync_DifferentDateFormats_ParsesCorrectly()
    {
        var csv = "data,quilometragem\n01/03/2024,50000\n2024-06-15,55000";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Equal(2, records.Count);
    }

    [Fact]
    public async Task ReadFromCsvFileAsync_FileNotFound_ThrowsFileNotFoundException()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _reader.ReadFromCsvFileAsync("nonexistent.csv", _vehicleId));
    }

    [Fact]
    public async Task ReadFromCsvFileAsync_ValidFile_ReturnsRecords()
    {
        var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "data", "mileage-sample.csv");
        
        if (File.Exists(csvPath))
        {
            var records = (await _reader.ReadFromCsvFileAsync(csvPath, _vehicleId)).ToList();

            Assert.Equal(10, records.Count);
            Assert.Equal(45000, records[0].Mileage);
            Assert.Equal(61500, records[^1].Mileage);
        }
    }

    [Fact]
    public async Task ReadFromCsvAsync_SetsCorrectVehicleId()
    {
        var csv = "data,quilometragem\n2024-01-01,50000";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Single(records);
        Assert.Equal(_vehicleId, records[0].VehicleId);
    }

    [Fact]
    public async Task ReadFromCsvAsync_MileageWithDots_ParsesCorrectly()
    {
        var csv = "data,quilometragem\n2024-01-01,50.000";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Single(records);
        Assert.Equal(50000, records[0].Mileage);
    }

    [Fact]
    public async Task ReadFromCsvAsync_ZeroMileage_SkipsRow()
    {
        var csv = "data,quilometragem\n2024-01-01,50000\n2024-06-01,0";
        
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        
        var records = (await _reader.ReadFromCsvAsync(stream, _vehicleId)).ToList();

        Assert.Single(records);
    }
}