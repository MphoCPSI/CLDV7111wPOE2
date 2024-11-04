using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Data.Data;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;

namespace KhumaloCraft.Data.Repositories.Implementations;

public class StatusRepository : IStatusRepository
{
  private readonly ApplicationDbContext _dbContext;

  public StatusRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<List<Status>> GetAllStatusAsync()
  {
    return await _dbContext.Status.ToListAsync();
  }

  public async Task<Status?> GetStatusByIdAsync(int statusId)
  {
    return await _dbContext.Status.FindAsync(statusId);
  }

  public async Task AddStatusAsync(Status status)
  {
    await _dbContext.Status.AddAsync(status);
  }

  public Task UpdateStatusAsync(Status status)
  {
    _dbContext.Status.Update(status);

    return Task.CompletedTask;
  }

  public Task DeleteStatusAsync(Status status)
  {
    _dbContext.Status.Remove(status);

    return Task.CompletedTask;
  }

  public async Task SaveChangesAsync()
  {
    await _dbContext.SaveChangesAsync();
  }
}
