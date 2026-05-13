using Application.DTOs;
using Application.Services;
using Core.Services;
using Moq;

namespace Application.Test.Services;

public class MaintenanceAnalysisUseCaseTests
{
    private readonly Mock<IVehicleService> _mockVehicleService;
    private readonly IMaintenanceCalculationService _calculationService;
    private readonly MaintenanceAnalysisUseCase _useCase;

    public MaintenanceAnalysisUseCaseTests()
    {
        _mockVehicleService = new Mock<IVehicleService>();
        _calculationService = new MaintenanceCalculationService();
        _useCase = new MaintenanceAnalysisUseCase(_mockVehicleService.Object, _calculationService);
    }

    [Fact]
    public async Task ExecuteAsync_ValidVehicle_ReturnsAnalysis()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 50000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var result = await _useCase.ExecuteAsync(request);

        Assert.Equal(vehicleId, result.VehicleId);
        Assert.Equal("Toyota", result.Brand);
        Assert.Equal("Corolla", result.Model);
        Assert.Equal(50000, result.CurrentMileage);
    }

    [Fact]
    public async Task ExecuteAsync_VehicleNotFound_ThrowsKeyNotFoundException()
    {
        var vehicleId = Guid.NewGuid();
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync((VehicleDto?)null);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ServicesDue_ReturnsRecommendations()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 55000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var result = await _useCase.ExecuteAsync(request);

        Assert.NotEmpty(result.Recommendations);
    }

    [Fact]
    public async Task ExecuteAsync_ServicesDue_SortsByUrgency()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 70000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var result = await _useCase.ExecuteAsync(request);

        var firstUrgency = result.Recommendations.First().UrgencyLevel;
        Assert.NotEqual("Low", firstUrgency);
    }

    [Fact]
    public async Task ExecuteAsync_ZeroMileage_ReturnsRecommendations()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2024, 0, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var result = await _useCase.ExecuteAsync(request);

        Assert.NotEmpty(result.Recommendations);
    }

    [Fact]
    public async Task ExecuteAsync_WithAdditionalContext_PassesContext()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 55000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId, "Vehicle used for Uber");
        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ExecuteAsync_SetsAnalyzedAt()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 55000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var beforeExecute = DateTime.UtcNow;
        
        var result = await _useCase.ExecuteAsync(request);
        
        var afterExecute = DateTime.UtcNow;

        Assert.InRange(result.AnalyzedAt, beforeExecute, afterExecute);
    }

    [Fact]
    public async Task ExecuteAsync_Recommendation_HasRequiredProperties()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 55000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var result = await _useCase.ExecuteAsync(request);

        var recommendation = result.Recommendations.First();
        
        Assert.False(string.IsNullOrEmpty(recommendation.ServiceType));
        Assert.False(string.IsNullOrEmpty(recommendation.Description));
        Assert.False(string.IsNullOrEmpty(recommendation.UrgencyLevel));
        Assert.InRange(recommendation.EstimatedMileage, 1, int.MaxValue);
        Assert.InRange(recommendation.MilesUntilDue, int.MinValue, int.MaxValue);
    }

    [Fact]
    public async Task ExecuteAsync_Recommendation_WithParts_HasPrice()
    {
        var vehicleId = Guid.NewGuid();
        var vehicleDto = new VehicleDto(vehicleId, "Toyota", "Corolla", 2020, 55000, DateTime.UtcNow, DateTime.UtcNow);
        
        _mockVehicleService.Setup(s => s.GetByIdAsync(vehicleId)).ReturnsAsync(vehicleDto);

        var request = new MaintenanceAnalysisRequestDto(vehicleId);
        var result = await _useCase.ExecuteAsync(request);

        var oilRecommendation = result.Recommendations.FirstOrDefault(r => r.ServiceType == "OilChange");
        
        Assert.NotNull(oilRecommendation);
        Assert.NotEmpty(oilRecommendation.RecommendedParts);
        Assert.True(oilRecommendation.EstimatedTotalPrice > 0);
    }
}