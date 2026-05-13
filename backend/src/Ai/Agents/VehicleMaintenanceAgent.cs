using Microsoft.Extensions.Logging;
using Core.Services;
using Core.Enums;

namespace Ai.Agents;

public interface IVehicleMaintenanceAgent
{
    Task<string> AnalyzeVehicleAsync(string prompt, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> AnalyzeVehicleStreamingAsync(string prompt, CancellationToken cancellationToken = default);
}

public class VehicleMaintenanceAgent : IVehicleMaintenanceAgent
{
    private readonly ILogger<VehicleMaintenanceAgent> _logger;
    private readonly IMaintenanceCalculationService _calculationService;
    private readonly string _instructions;

    public VehicleMaintenanceAgent(
        IMaintenanceCalculationService calculationService,
        ILogger<VehicleMaintenanceAgent> logger)
    {
        _calculationService = calculationService;
        _logger = logger;

        _instructions = """
            Você é um especialista em manutenção de veículos.
            
            Analise os dados do veículo e forneça recomendações personalizadas de manutenção.
            
            Forneça respostas claras e úteis em português brasileiro.
            Sempre inclua informações sobre peças necessárias e custo estimado quando aplicável.
            """;
    }

    public int CalculateNextOilChange(int currentMileage)
    {
        _logger.LogDebug("Calculando próxima troca de óleo para {Mileage}km", currentMileage);
        return _calculationService.CalculateNextOilChange(currentMileage);
    }

    public int CalculateNextTireReplacement(int currentMileage)
    {
        _logger.LogDebug("Calculando próxima troca de pneus para {Mileage}km", currentMileage);
        return _calculationService.CalculateNextTireReplacement(currentMileage);
    }

    public string GetRecommendedParts(string serviceType)
    {
        _logger.LogDebug("Buscando peças para serviço: {ServiceType}", serviceType);
        
        if (!Enum.TryParse<MaintenanceType>(serviceType, true, out var type))
            return "Tipo de serviço inválido";

        var parts = _calculationService.GetRecommendedParts(type);
        
        if (!parts.Any())
            return "Nenhuma peça necessária para este serviço";

        var result = string.Join("\n", parts.Select(p => 
            $"- {p.Name}: {p.Quantity}x - R$ {p.EstimatedPrice:F2} (Código: {p.PartNumber ?? "N/A"})"));
        
        return $"Peças necessárias:\n{result}\n\nTotal estimado: R$ {parts.Sum(p => p.TotalPrice):F2}";
    }

    public string CalculateUrgency(int currentMileage, int lastServiceMileage, string serviceType)
    {
        _logger.LogDebug("Calculando urgência para {ServiceType}", serviceType);
        
        if (!Enum.TryParse<MaintenanceType>(serviceType, true, out var type))
            return "Tipo de serviço inválido";

        var urgency = _calculationService.CalculateUrgency(currentMileage, lastServiceMileage, type);
        
        return urgency switch
        {
            UrgencyLevel.Low => "Baixa - Pode esperar",
            UrgencyLevel.Medium => "Média - Agendar nas próximas semanas",
            UrgencyLevel.High => "Alta - Fazer em breve",
            UrgencyLevel.Critical => "Crítica - Não espere",
            _ => "Desconhecido"
        };
    }

    public Task<string> AnalyzeVehicleAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("AI Agent requer configuração. Configure a API Key no appsettings.json");
        
        var response = _instructions + @"


---

⚠️ Para ativar o agente de IA, configure a API Key no appsettings.json:

```json
{
  ""AI"": {
    ""ApiKey"": ""sua-api-key-aqui""
  }
}
```

Prompt recebido: " + prompt;
        
        return Task.FromResult(response);
    }

    public async IAsyncEnumerable<string> AnalyzeVehicleStreamingAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await AnalyzeVehicleAsync(prompt, cancellationToken);
        yield return response;
    }
}