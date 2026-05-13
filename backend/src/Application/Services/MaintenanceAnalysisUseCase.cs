using Application.DTOs;
using Core.Enums;
using Core.Services;

namespace Application.Services;

public class MaintenanceAnalysisUseCase : IMaintenanceAnalysisUseCase
{
    private readonly IVehicleService _vehicleService;
    private readonly IMaintenanceCalculationService _calculationService;

    public MaintenanceAnalysisUseCase(
        IVehicleService vehicleService,
        IMaintenanceCalculationService calculationService)
    {
        _vehicleService = vehicleService;
        _calculationService = calculationService;
    }

    public async Task<MaintenanceAnalysisResponseDto> ExecuteAsync(
        MaintenanceAnalysisRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleService.GetByIdAsync(request.VehicleId);
        
        if (vehicle == null)
            throw new KeyNotFoundException($"Vehicle with id {request.VehicleId} not found");

        var recommendations = new List<MaintenanceRecommendationDto>();

        var maintenanceTypes = new[]
        {
            MaintenanceType.OilChange,
            MaintenanceType.TireRotation,
            MaintenanceType.TireReplacement,
            MaintenanceType.BrakeService,
            MaintenanceType.GeneralInspection,
            MaintenanceType.AirFilter
        };

        foreach (var type in maintenanceTypes)
        {
            var isDue = _calculationService.IsServiceDue(vehicle.CurrentMileage, 0, type);
            
            if (isDue)
            {
                var nextKm = _calculationService.CalculateNextMaintenance(vehicle.CurrentMileage, type);
                var urgency = _calculationService.CalculateUrgency(vehicle.CurrentMileage, 0, type);
                var parts = _calculationService.GetRecommendedParts(type);
                var totalPrice = parts.Sum(p => p.TotalPrice);

                recommendations.Add(new MaintenanceRecommendationDto(
                    type.ToString(),
                    GetDescription(type),
                    urgency.ToString(),
                    nextKm,
                    nextKm - vehicle.CurrentMileage,
                    parts.Select(p => new PartDto(p.Name, p.Quantity, p.EstimatedPrice, p.PartNumber)).ToList(),
                    totalPrice
                ));
            }
        }

        var sortedRecommendations = recommendations
            .OrderBy(r => r.UrgencyLevel switch
            {
                "Critical" => 0,
                "High" => 1,
                "Medium" => 2,
                "Low" => 3,
                _ => 4
            })
            .ThenBy(r => r.MilesUntilDue)
            .ToList();

        return new MaintenanceAnalysisResponseDto(
            vehicle.Id,
            vehicle.Brand,
            vehicle.Model,
            vehicle.CurrentMileage,
            sortedRecommendations,
            DateTime.UtcNow
        );
    }

    private static string GetDescription(MaintenanceType type) => type switch
    {
        MaintenanceType.OilChange => "Troca de óleo e filtro",
        MaintenanceType.TireRotation => "Rodízio de pneus",
        MaintenanceType.TireReplacement => "Troca de pneus",
        MaintenanceType.BrakeService => "Serviço de freios",
        MaintenanceType.GeneralInspection => "Revisão geral",
        MaintenanceType.AirFilter => "Troca do filtro de ar",
        MaintenanceType.TransmissionService => "Serviço de transmissão",
        MaintenanceType.CoolantFlush => "Troca de líquido de arrefecimento",
        _ => type.ToString()
    };
}