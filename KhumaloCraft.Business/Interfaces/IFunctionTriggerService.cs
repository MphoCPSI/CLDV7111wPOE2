using KhumaloCraft.Shared.DTOs;
using KhumaloCraft.Shared.Helpers;

namespace KhumaloCraft.Business.Interfaces;

public interface IFunctionTriggerService
{
    Task<Response<string>> StartOrderProcessingOrchestratorAsync(CartRequestDTO requestDTO);
    Task<Response<string>> StartNotificationsOrchestratorAsync(NotificationRequest requestDTO);
    Task<Response<string>> StartProductNotificationsOrchestratorAsync(ProductNotificationsRequest requestDTO);
}
