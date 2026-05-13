using Core.Entities;

namespace Core.Interfaces;

public interface IMileageDataReader
{
    Task<IEnumerable<MileageRecord>> ReadFromCsvAsync(Stream csvStream, Guid vehicleId);
    Task<IEnumerable<MileageRecord>> ReadFromCsvFileAsync(string filePath, Guid vehicleId);
}