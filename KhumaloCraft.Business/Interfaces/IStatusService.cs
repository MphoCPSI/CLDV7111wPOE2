using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Interfaces;

public interface IStatusService
{
  Task<List<StatusDTO>> GetAllStatus();
  Task<StatusDTO?> GetStatusById(int statusId);
  Task AddStatus(StatusDTO statusDTO);
  Task UpdateStatus(StatusDTO statusDTO);
  Task DeleteStatus(int statusId);
}
