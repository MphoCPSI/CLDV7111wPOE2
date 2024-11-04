using Microsoft.EntityFrameworkCore;
using KhumaloCraft.Data.Data;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Repositories.Interfaces;

namespace KhumaloCraft.Data.Repositories.Implementations;

public class UserRepository : IUserRepository
{
  private readonly ApplicationDbContext _dbContext;

  public UserRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<List<string>> GetAllUsersIdsAsync()
  {
    return await _dbContext.Users.Select(u => u.Id).ToListAsync();
  }

  public async Task SaveChangesAsync()
  {
    await _dbContext.SaveChangesAsync();
  }
}
