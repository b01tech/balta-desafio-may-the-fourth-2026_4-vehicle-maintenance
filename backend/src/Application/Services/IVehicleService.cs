using Application.DTOs;
using Core.Entities;

namespace Application.Services;

public interface IVehicleService
{
    Task<VehicleDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<VehicleDto>> GetAllAsync();
    Task<VehicleDto> CreateAsync(CreateVehicleDto dto);
    Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<MileageRecord>> GetMileageRecordsAsync(Guid vehicleId);
    Task ImportMileageFromCsvAsync(Guid vehicleId, Stream csvStream);
}