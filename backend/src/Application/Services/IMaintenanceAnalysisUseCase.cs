using Application.DTOs;

namespace Application.Services;

public interface IMaintenanceAnalysisUseCase
{
    Task<MaintenanceAnalysisResponseDto> ExecuteAsync(MaintenanceAnalysisRequestDto request, CancellationToken cancellationToken = default);
}