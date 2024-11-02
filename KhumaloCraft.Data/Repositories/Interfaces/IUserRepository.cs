using KhumaloCraft.Data.Entities;

namespace KhumaloCraft.Data.Repositories.Interfaces;

public interface IUserRepository
{
  Task<List<string>> GetAllUsersIdsAsync();
}
