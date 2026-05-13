using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Services;
using Ai.Agents;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IMaintenanceAnalysisUseCase _maintenanceAnalysisUseCase;
    private readonly IVehicleMaintenanceAgent _agent;

    public VehicleController(
        IVehicleService vehicleService,
        IMaintenanceAnalysisUseCase maintenanceAnalysisUseCase,
        IVehicleMaintenanceAgent agent)
    {
        _vehicleService = vehicleService;
        _maintenanceAnalysisUseCase = maintenanceAnalysisUseCase;
        _agent = agent;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll()
    {
        var vehicles = await _vehicleService.GetAllAsync();
        return Ok(vehicles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetById(Guid id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        
        if (vehicle == null)
            return NotFound(new { message = $"Vehicle with id {id} not found" });

        return Ok(vehicle);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleDto dto)
    {
        var vehicle = await _vehicleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleDto>> Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        try
        {
            var vehicle = await _vehicleService.UpdateAsync(id, dto);
            return Ok(vehicle);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Vehicle with id {id} not found" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _vehicleService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Vehicle with id {id} not found" });

        return NoContent();
    }

    [HttpPost("{id}/upload-csv")]
    public async Task<ActionResult> UploadCsv(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "File is required" });

        if (!file.ContentType.Contains("csv") && !file.FileName.EndsWith(".csv"))
            return BadRequest(new { message = "Only CSV files are allowed" });

        try
        {
            await using var stream = file.OpenReadStream();
            await _vehicleService.ImportMileageFromCsvAsync(id, stream);
            return Ok(new { message = "CSV imported successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Vehicle with id {id} not found" });
        }
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<MaintenanceAnalysisResponseDto>> Analyze([FromBody] MaintenanceAnalysisRequestDto request)
    {
        try
        {
            var result = await _maintenanceAnalysisUseCase.ExecuteAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Vehicle with id {request.VehicleId} not found" });
        }
    }

    [HttpPost("analyze-ai")]
    public async Task<ActionResult<string>> AnalyzeWithAI([FromBody] AiAnalysisRequestDto request)
    {
        var vehicle = await _vehicleService.GetByIdAsync(request.VehicleId);
        
        if (vehicle == null)
            return NotFound(new { message = $"Vehicle with id {request.VehicleId} not found" });

        var prompt = $"""
            Analise a manutenção do veículo:
            
            - Marca: {vehicle.Brand}
            - Modelo: {vehicle.Model}
            - Ano: {vehicle.Year}
            - Quilometragem atual: {vehicle.CurrentMileage} km
            
            {request.AdditionalContext ?? "Forneça recomendações gerais de manutenção."}
            """;

        var result = await _agent.AnalyzeVehicleAsync(prompt);
        return Ok(new { analysis = result });
    }

    [HttpPost("analyze-ai/stream")]
    public async Task AnalyzeWithAIStreaming([FromBody] AiAnalysisRequestDto request)
    {
        var vehicle = await _vehicleService.GetByIdAsync(request.VehicleId);
        
        if (vehicle == null)
        {
            Response.StatusCode = 404;
            return;
        }

        var prompt = $"""
            Analise a manutenção do veículo:
            
            - Marca: {vehicle.Brand}
            - Modelo: {vehicle.Model}
            - Ano: {vehicle.Year}
            - Quilometragem atual: {vehicle.CurrentMileage} km
            
            {request.AdditionalContext ?? "Forneça recomendações gerais de manutenção."}
            """;

        Response.ContentType = "text/plain";
        
        await foreach (var chunk in _agent.AnalyzeVehicleStreamingAsync(prompt))
        {
            await Response.WriteAsync(chunk);
            await Response.Body.FlushAsync();
        }
    }
}

public record AiAnalysisRequestDto(Guid VehicleId, string? AdditionalContext = null);