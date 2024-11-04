using KhumaloCraft.Data.Entities;

namespace KhumaloCraft.Data.Repositories.Interfaces;

public interface IStatusRepository
{
  Task<List<Status>> GetAllStatusAsync();
  Task<Status?> GetStatusByIdAsync(int statusId);
  Task AddStatusAsync(Status status);
  Task UpdateStatusAsync(Status status);
  Task DeleteStatusAsync(Status status);
  Task SaveChangesAsync();
}
