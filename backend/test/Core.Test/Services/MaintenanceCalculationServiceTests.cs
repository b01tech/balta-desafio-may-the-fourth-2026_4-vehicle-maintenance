using Core.Enums;
using Core.Services;

namespace Core.Test.Services;

public class MaintenanceCalculationServiceTests
{
    private readonly MaintenanceCalculationService _service = new();

    [Theory]
    [InlineData(0, 5000)]
    [InlineData(5000, 10000)]
    [InlineData(12500, 15000)]
    [InlineData(49000, 50000)]
    [InlineData(50000, 55000)]
    public void CalculateNextOilChange_Returns5000KmAhead(int currentMileage, int expected)
    {
        var result = _service.CalculateNextOilChange(currentMileage);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 10000)]
    [InlineData(10000, 20000)]
    [InlineData(15000, 20000)]
    [InlineData(95000, 100000)]
    public void CalculateNextTireRotation_Returns10000KmAhead(int currentMileage, int expected)
    {
        var result = _service.CalculateNextTireRotation(currentMileage);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 40000)]
    [InlineData(40000, 80000)]
    [InlineData(35000, 40000)]
    [InlineData(120000, 160000)]
    public void CalculateNextTireReplacement_Returns40000KmAhead(int currentMileage, int expected)
    {
        var result = _service.CalculateNextTireReplacement(currentMileage);
        
        Assert.Equal(expected, result);
    }

[Fact]
    public void CalculateUrgency_OilChangeOverdue_ReturnsCritical()
    {
        var result = _service.CalculateUrgency(57000, 46000, MaintenanceType.OilChange);
        
        Assert.Equal(UrgencyLevel.Critical, result);
    }

    [Fact]
    public void CalculateUrgency_OilChangeDueSoon_ReturnsMedium()
    {
        var result = _service.CalculateUrgency(51000, 47500, MaintenanceType.OilChange);
        
        Assert.Equal(UrgencyLevel.Medium, result);
    }

    [Fact]
    public void CalculateUrgency_OilChangeDueSoon2_ReturnsMedium()
    {
        var result = _service.CalculateUrgency(49000, 45000, MaintenanceType.OilChange);
        
        Assert.Equal(UrgencyLevel.Medium, result);
    }

    [Fact]
    public void CalculateUrgency_OilChangeOk_ReturnsLow()
    {
        var result = _service.CalculateUrgency(22000, 20000, MaintenanceType.OilChange);
        
        Assert.Equal(UrgencyLevel.Low, result);
    }

    [Fact]
    public void CalculateUrgency_NoPreviousService_ReturnsCritical()
    {
        var result = _service.CalculateUrgency(30000, 0, MaintenanceType.OilChange);
        
        Assert.Equal(UrgencyLevel.Critical, result);
    }

    [Fact]
    public void GetRecommendedParts_OilChange_ReturnsOilAndFilter()
    {
        var result = _service.GetRecommendedParts(MaintenanceType.OilChange);
        
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name.Contains("Óleo"));
        Assert.Contains(result, p => p.Name.Contains("Filtro"));
    }

    [Fact]
    public void GetRecommendedParts_TireReplacement_ReturnsTires()
    {
        var result = _service.GetRecommendedParts(MaintenanceType.TireReplacement);
        
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name.Contains("Pneu"));
    }

    [Fact]
    public void GetRecommendedParts_TireRotation_ReturnsEmptyList()
    {
        var result = _service.GetRecommendedParts(MaintenanceType.TireRotation);
        
        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommendedParts_BrakeService_ReturnsBrakeParts()
    {
        var result = _service.GetRecommendedParts(MaintenanceType.BrakeService);
        
        Assert.Equal(4, result.Count);
        Assert.Contains(result, p => p.Name.Contains("Pastilha"));
    }

    [Theory]
    [InlineData(51000, 46000, true)]
    [InlineData(55000, 50000, true)]
    [InlineData(49000, 46000, false)]
    [InlineData(45000, 46000, false)]
    public void IsServiceDue_Overdue_ReturnsTrue(int currentMileage, int lastServiceMileage, bool expected)
    {
        var result = _service.IsServiceDue(currentMileage, lastServiceMileage, MaintenanceType.OilChange);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsServiceDue_NoPreviousService_ReturnsTrue()
    {
        var result = _service.IsServiceDue(3000, 0, MaintenanceType.OilChange);
        
        Assert.True(result);
    }

    [Fact]
    public void CalculateNextGeneralInspection_Returns10000KmAhead()
    {
        var result = _service.CalculateNextGeneralInspection(5000);
        
        Assert.Equal(10000, result);
    }

    [Fact]
    public void CalculateNextMaintenance_AirFilter_Returns15000KmAhead()
    {
        var result = _service.CalculateNextMaintenance(0, MaintenanceType.AirFilter);
        
        Assert.Equal(15000, result);
    }

    [Fact]
    public void CalculateNextMaintenance_TransmissionService_Returns60000KmAhead()
    {
        var result = _service.CalculateNextMaintenance(0, MaintenanceType.TransmissionService);
        
        Assert.Equal(60000, result);
    }

    [Fact]
    public void GetRecommendedParts_GeneralInspection_ReturnsFilters()
    {
        var result = _service.GetRecommendedParts(MaintenanceType.GeneralInspection);
        
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name.Contains("Filtro"));
    }

    [Fact]
    public void GetRecommendedParts_CoolantFlush_ReturnsCoolantParts()
    {
        var result = _service.GetRecommendedParts(MaintenanceType.CoolantFlush);
        
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name.Contains("Líquido") || p.Name.Contains("Termostato"));
    }
}