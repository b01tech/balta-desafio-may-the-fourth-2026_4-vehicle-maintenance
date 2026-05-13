using Core.Entities;
using Core.Enums;

namespace Core.Services;

public static class MaintenanceIntervals
{
    public const int OilChange = 5000;
    public const int TireRotation = 10000;
    public const int TireReplacement = 40000;
    public const int GeneralInspection = 10000;
    public const int AirFilter = 15000;
    public const int BrakeInspection = 20000;
    public const int TransmissionService = 60000;
    public const int CoolantFlush = 30000;
}

public interface IMaintenanceCalculationService
{
    int CalculateNextOilChange(int currentMileage);
    int CalculateNextTireRotation(int currentMileage);
    int CalculateNextTireReplacement(int currentMileage);
    int CalculateNextGeneralInspection(int currentMileage);
    int CalculateNextMaintenance(int currentMileage, MaintenanceType type);
    UrgencyLevel CalculateUrgency(int currentMileage, int lastServiceMileage, MaintenanceType type);
    IReadOnlyList<MaintenancePart> GetRecommendedParts(MaintenanceType type);
    bool IsServiceDue(int currentMileage, int lastServiceMileage, MaintenanceType type);
}

public class MaintenanceCalculationService : IMaintenanceCalculationService
{
    public int CalculateNextOilChange(int currentMileage)
        => CalculateNextMaintenance(currentMileage, MaintenanceType.OilChange);

    public int CalculateNextTireRotation(int currentMileage)
        => CalculateNextMaintenance(currentMileage, MaintenanceType.TireRotation);

    public int CalculateNextTireReplacement(int currentMileage)
        => CalculateNextMaintenance(currentMileage, MaintenanceType.TireReplacement);

    public int CalculateNextGeneralInspection(int currentMileage)
        => CalculateNextMaintenance(currentMileage, MaintenanceType.GeneralInspection);

    public int CalculateNextMaintenance(int currentMileage, MaintenanceType type)
    {
        int interval = type switch
        {
            MaintenanceType.OilChange => MaintenanceIntervals.OilChange,
            MaintenanceType.TireRotation => MaintenanceIntervals.TireRotation,
            MaintenanceType.TireReplacement => MaintenanceIntervals.TireReplacement,
            MaintenanceType.GeneralInspection => MaintenanceIntervals.GeneralInspection,
            MaintenanceType.AirFilter => MaintenanceIntervals.AirFilter,
            MaintenanceType.BrakeService => MaintenanceIntervals.BrakeInspection,
            MaintenanceType.TransmissionService => MaintenanceIntervals.TransmissionService,
            MaintenanceType.CoolantFlush => MaintenanceIntervals.CoolantFlush,
            _ => MaintenanceIntervals.GeneralInspection
        };

        int nextService = ((currentMileage / interval) + 1) * interval;
        return nextService;
    }

    public UrgencyLevel CalculateUrgency(int currentMileage, int lastServiceMileage, MaintenanceType type)
    {
        if (lastServiceMileage <= 0)
        {
            int suggestedInterval = type switch
            {
                MaintenanceType.OilChange => MaintenanceIntervals.OilChange,
                MaintenanceType.TireRotation => MaintenanceIntervals.TireRotation,
                MaintenanceType.TireReplacement => MaintenanceIntervals.TireReplacement,
                MaintenanceType.GeneralInspection => MaintenanceIntervals.GeneralInspection,
                MaintenanceType.AirFilter => MaintenanceIntervals.AirFilter,
                MaintenanceType.BrakeService => MaintenanceIntervals.BrakeInspection,
                MaintenanceType.TransmissionService => MaintenanceIntervals.TransmissionService,
                MaintenanceType.CoolantFlush => MaintenanceIntervals.CoolantFlush,
                _ => MaintenanceIntervals.GeneralInspection
            };
            
            if (currentMileage >= suggestedInterval)
                return UrgencyLevel.Critical;
            else if (currentMileage >= suggestedInterval * 0.8m)
                return UrgencyLevel.High;
            else if (currentMileage >= suggestedInterval * 0.5m)
                return UrgencyLevel.Medium;
            else
                return UrgencyLevel.Low;
        }

        int interval = type switch
        {
            MaintenanceType.OilChange => MaintenanceIntervals.OilChange,
            MaintenanceType.TireRotation => MaintenanceIntervals.TireRotation,
            MaintenanceType.TireReplacement => MaintenanceIntervals.TireReplacement,
            MaintenanceType.GeneralInspection => MaintenanceIntervals.GeneralInspection,
            MaintenanceType.AirFilter => MaintenanceIntervals.AirFilter,
            MaintenanceType.BrakeService => MaintenanceIntervals.BrakeInspection,
            MaintenanceType.TransmissionService => MaintenanceIntervals.TransmissionService,
            MaintenanceType.CoolantFlush => MaintenanceIntervals.CoolantFlush,
            _ => MaintenanceIntervals.GeneralInspection
        };

        int nextService = lastServiceMileage + interval;
        int overdueBy = currentMileage - nextService;

        if (overdueBy <= -interval * 0.5m)
            return UrgencyLevel.Low;
        else if (overdueBy <= 0)
            return UrgencyLevel.Medium;
        else if (overdueBy <= interval * 0.25m)
            return UrgencyLevel.High;
        else
            return UrgencyLevel.Critical;
    }

    public IReadOnlyList<MaintenancePart> GetRecommendedParts(MaintenanceType type)
    {
        return type switch
        {
            MaintenanceType.OilChange => new List<MaintenancePart>
            {
                new("Óleo Motor 5W-30", 1, 89.90m, "OEM-5W30"),
                new("Filtro de Óleo", 1, 45.00m, "FO-001"),
                new("Anel de Vedação do Carter", 1, 12.00m, "AV-CARTER")
            },
            MaintenanceType.TireRotation => new List<MaintenancePart>(),
            MaintenanceType.TireReplacement => new List<MaintenancePart>
            {
                new("Pneu Dianteiro (unitário)", 2, 450.00m, "PNEU-Dianteiro"),
                new("Pneu Traseiro (unitário)", 2, 450.00m, "PNEU-Traseiro"),
                new("Válvula de Pneu", 4, 15.00m, "VALV-Pneu")
            },
            MaintenanceType.BrakeService => new List<MaintenancePart>
            {
                new("Pastilha de Freio Dianteira", 2, 180.00m, "PFD-001"),
                new("Pastilha de Freio Traseira", 2, 150.00m, "PFT-001"),
                new("Disco de Freio Dianteiro", 2, 220.00m, "DFD-001"),
                new("Fluido de Freio DOT4", 1, 65.00m, "FLU-DOT4")
            },
            MaintenanceType.GeneralInspection => new List<MaintenancePart>
            {
                new("Filtro de Ar", 1, 55.00m, "FA-001"),
                new("Filtro de Combustível", 1, 85.00m, "FC-001"),
                new("Filtro de Ar Condicionado", 1, 75.00m, "FAC-001")
            },
            MaintenanceType.AirFilter => new List<MaintenancePart>
            {
                new("Filtro de Ar", 1, 55.00m, "FA-001")
            },
            MaintenanceType.TransmissionService => new List<MaintenancePart>
            {
                new("Óleo de Transmissão ATF", 4, 45.00m, "ATF-001"),
                new("Filtro de Transmissão", 1, 120.00m, "FT-001"),
                new("Junta do Carter", 1, 35.00m, "JC-001")
            },
            MaintenanceType.CoolantFlush => new List<MaintenancePart>
            {
                new("Líquido de Arrefecimento", 5, 38.00m, "LA-001"),
                new("Termostato", 1, 95.00m, "TERM-001")
            },
            _ => new List<MaintenancePart>()
        };
    }

    public bool IsServiceDue(int currentMileage, int lastServiceMileage, MaintenanceType type)
    {
        if (lastServiceMileage <= 0)
            return true;

        int interval = type switch
        {
            MaintenanceType.OilChange => MaintenanceIntervals.OilChange,
            MaintenanceType.TireRotation => MaintenanceIntervals.TireRotation,
            MaintenanceType.TireReplacement => MaintenanceIntervals.TireReplacement,
            MaintenanceType.GeneralInspection => MaintenanceIntervals.GeneralInspection,
            MaintenanceType.AirFilter => MaintenanceIntervals.AirFilter,
            MaintenanceType.BrakeService => MaintenanceIntervals.BrakeInspection,
            MaintenanceType.TransmissionService => MaintenanceIntervals.TransmissionService,
            MaintenanceType.CoolantFlush => MaintenanceIntervals.CoolantFlush,
            _ => MaintenanceIntervals.GeneralInspection
        };

        int nextService = lastServiceMileage + interval;
        return currentMileage >= nextService;
    }
}