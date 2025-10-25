using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories
{
  public class InMemoryUserRepository : IUserRepository
  {
    private readonly ConcurrentDictionary<Guid, User> _store = new();

    public InMemoryUserRepository()
    {
      // Seed a sample user
      var u = new User
      {
        Id = Guid.NewGuid(),
        FirstName = "Admin",
        LastName = "User",
        Email = "admin@techhive.local"
      };
      _store[u.Id] = u;
    }

    public IEnumerable<User> GetAll() => _store.Values;

    public User? Get(Guid id) => _store.TryGetValue(id, out var u) ? u : null;

    public void Add(User user) => _store[user.Id] = user;

    public void Update(User user) => _store[user.Id] = user;

    public void Delete(Guid id) => _store.TryRemove(id, out _);
  }
}
