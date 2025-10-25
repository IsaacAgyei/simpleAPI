using System;
using System.Collections.Generic;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories
{
  public interface IUserRepository
  {
    IEnumerable<User> GetAll();
    User? Get(Guid id);
    void Add(User user);
    void Update(User user);
    void Delete(Guid id);
  }
}
