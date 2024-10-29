using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface IFunctionTriggerService
{
    Task<string> StartOrderProcessingOrchestratorAsync(OrderDTO orderDTO);
}
