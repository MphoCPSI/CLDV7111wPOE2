using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;
using KhumaloCraft.Shared.DTOs;

namespace KhumaloCraft.Business.Services;

public class StatusService : IStatusService
{
  private readonly IStatusRepository _statusRepository;

  public StatusService(IStatusRepository statusRepository)
  {
    _statusRepository = statusRepository;
  }

  public async Task<List<StatusDTO>> GetAllStatus()
  {
    var status = await _statusRepository.GetAllStatusAsync();
    return status.Select(c => new StatusDTO
    {
      StatusId = c.StatusId,
      StatusName = c.StatusName
    }).ToList();
  }

  public async Task<StatusDTO?> GetStatusById(int statusId)
  {
    var status = await _statusRepository.GetStatusByIdAsync(statusId);
    if (status == null) return null;

    return new StatusDTO
    {
      StatusId = status.StatusId,
      StatusName = status.StatusName
    };
  }

  public async Task AddStatus(StatusDTO statusDTO)
  {
    var status = new Status
    {
      StatusName = statusDTO.StatusName
    };

    await _statusRepository.AddStatusAsync(status);
    await _statusRepository.SaveChangesAsync();
  }

  public async Task UpdateStatus(StatusDTO statusDTO)
  {
    var status = await _statusRepository.GetStatusByIdAsync(statusDTO.StatusId);
    if (status == null) return;

    status.StatusName = statusDTO.StatusName;

    await _statusRepository.UpdateStatusAsync(status);
    await _statusRepository.SaveChangesAsync();
  }

  public async Task DeleteStatus(int statusId)
  {
    var status = await _statusRepository.GetStatusByIdAsync(statusId);
    if (status == null) return;

    await _statusRepository.DeleteStatusAsync(status);
    await _statusRepository.SaveChangesAsync();
  }
}
