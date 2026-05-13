using System.Globalization;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Readers;

public class CsvMileageDataReader : IMileageDataReader
{
    private const char DefaultDelimiter = ',';
    private const int ExpectedColumns = 2;

    public async Task<IEnumerable<MileageRecord>> ReadFromCsvAsync(Stream csvStream, Guid vehicleId)
    {
        var records = new List<MileageRecord>();

        using var reader = new StreamReader(csvStream, leaveOpen: true);
        
        var header = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(header))
            return records;

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var record = ParseLine(line, vehicleId);
            if (record != null)
                records.Add(record);
        }

        return records.OrderBy(r => r.Date);
    }

    public async Task<IEnumerable<MileageRecord>> ReadFromCsvFileAsync(string filePath, Guid vehicleId)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await ReadFromCsvAsync(stream, vehicleId);
    }

    private MileageRecord? ParseLine(string line, Guid vehicleId)
    {
        var columns = line.Split(DefaultDelimiter);

        if (columns.Length < ExpectedColumns)
            return null;

        var dateStr = columns[0].Trim();
        var mileageStr = columns[1].Trim();

        if (!TryParseDate(dateStr, out var date))
            return null;

        if (!TryParseMileage(mileageStr, out var mileage))
            return null;

        try
        {
            return new MileageRecord(vehicleId, date, mileage);
        }
        catch
        {
            return null;
        }
    }

    private static bool TryParseDate(string value, out DateTime date)
    {
        date = default;
        
        var formats = new[]
        {
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "yyyy/MM/dd",
            "dd-MM-yyyy"
        };

        return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    private static bool TryParseMileage(string value, out int mileage)
    {
        mileage = 0;
        
        var cleanedValue = value.Replace(".", "").Replace(",", "").Trim();
        
        return int.TryParse(cleanedValue, out mileage) && mileage > 0;
    }
}