namespace Application.DTOs;

public record VehicleDto(
    Guid Id,
    string Brand,
    string Model,
    int Year,
    int CurrentMileage,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateVehicleDto(
    string Brand,
    string Model,
    int Year,
    int CurrentMileage
);

public record UpdateVehicleDto(
    string Brand,
    string Model,
    int Year,
    int CurrentMileage
);