using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

public class VehicleService : IVehicleService
{
    private readonly Dictionary<Guid, Vehicle> _vehicles = new();
    private readonly IMileageDataReader _mileageDataReader;

    public VehicleService(IMileageDataReader mileageDataReader)
    {
        _mileageDataReader = mileageDataReader;
    }

    public Task<VehicleDto?> GetByIdAsync(Guid id)
    {
        if (!_vehicles.TryGetValue(id, out var vehicle))
            return Task.FromResult<VehicleDto?>(null);

        return Task.FromResult<VehicleDto?>(MapToDto(vehicle));
    }

    public Task<IEnumerable<VehicleDto>> GetAllAsync()
    {
        var vehicles = _vehicles.Values.Select(MapToDto);
        return Task.FromResult(vehicles);
    }

    public Task<VehicleDto> CreateAsync(CreateVehicleDto dto)
    {
        var vehicle = new Vehicle(dto.Brand, dto.Model, dto.Year, dto.CurrentMileage);
        _vehicles[vehicle.Id] = vehicle;
        return Task.FromResult(MapToDto(vehicle));
    }

    public Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto dto)
    {
        if (!_vehicles.TryGetValue(id, out var vehicle))
            throw new KeyNotFoundException($"Vehicle with id {id} not found");

        vehicle.Update(dto.Brand, dto.Model, dto.Year);
        vehicle.UpdateMileage(dto.CurrentMileage);

        return Task.FromResult(MapToDto(vehicle));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_vehicles.Remove(id));
    }

    public Task<IEnumerable<MileageRecord>> GetMileageRecordsAsync(Guid vehicleId)
    {
        if (!_vehicles.TryGetValue(vehicleId, out var vehicle))
            throw new KeyNotFoundException($"Vehicle with id {vehicleId} not found");

        return Task.FromResult(Enumerable.Empty<MileageRecord>());
    }

    public async Task ImportMileageFromCsvAsync(Guid vehicleId, Stream csvStream)
    {
        if (!_vehicles.ContainsKey(vehicleId))
            throw new KeyNotFoundException($"Vehicle with id {vehicleId} not found");

        var records = await _mileageDataReader.ReadFromCsvAsync(csvStream, vehicleId);
        
        var vehicle = _vehicles[vehicleId];
        var lastRecord = records.LastOrDefault();
        
        if (lastRecord != null && lastRecord.Mileage > vehicle.CurrentMileage)
        {
            vehicle.UpdateMileage(lastRecord.Mileage);
        }
    }

    private static VehicleDto MapToDto(Vehicle vehicle) => new(
        vehicle.Id,
        vehicle.Brand,
        vehicle.Model,
        vehicle.Year,
        vehicle.CurrentMileage,
        vehicle.CreatedAt,
        vehicle.UpdatedAt
    );
}