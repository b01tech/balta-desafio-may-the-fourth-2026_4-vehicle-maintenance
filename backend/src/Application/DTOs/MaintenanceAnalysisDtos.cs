namespace Application.DTOs;

public record MaintenanceAnalysisRequestDto(
    Guid VehicleId,
    string? AdditionalContext = null
);

public record MaintenanceAnalysisResponseDto(
    Guid VehicleId,
    string Brand,
    string Model,
    int Year,
    int CurrentMileage,
    List<MaintenanceRecommendationDto> Recommendations,
    DateTime AnalyzedAt
);

public record MaintenanceRecommendationDto(
    string ServiceType,
    string Description,
    string UrgencyLevel,
    int EstimatedMileage,
    int MilesUntilDue,
    List<PartDto> RecommendedParts,
    decimal EstimatedTotalPrice
);

public record PartDto(
    string Name,
    int Quantity,
    decimal EstimatedPrice,
    string? PartNumber
);