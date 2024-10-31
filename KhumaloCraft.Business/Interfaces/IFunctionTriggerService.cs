using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface IFunctionTriggerService
{
    Task<Response<string>> StartOrderProcessingOrchestratorAsync(CartRequestDTO requestDTO);
}
