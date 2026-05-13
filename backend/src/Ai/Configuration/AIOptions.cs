namespace Ai.Configuration;

public class AIOptions
{
    public const string SectionName = "AI";
    
    public string Provider { get; set; } = "openrouter";
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "microsoft/phi-4-mini";
    public string Endpoint { get; set; } = "https://openrouter.ai/api/v1";
    public float Temperature { get; set; } = 0.7f;
}