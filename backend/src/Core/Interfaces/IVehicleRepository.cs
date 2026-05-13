using Core.Entities;

namespace Core.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<Vehicle?> GetByIdWithRecordsAsync(Guid id);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task<Vehicle> UpdateAsync(Vehicle vehicle);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<MileageRecord>> GetMileageRecordsAsync(Guid vehicleId);
    Task AddMileageRecordAsync(MileageRecord record);
}