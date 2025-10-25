using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
      _repo = repo;
    }

    // GET: api/users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
      return Ok(_repo.GetAll());
    }

    // GET: api/users/{id}
    [HttpGet("{id}", Name = "GetUser")]
    public ActionResult<User> Get(Guid id)
    {
      var user = _repo.Get(id);
      if (user == null) return NotFound();
      return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public ActionResult<User> Create([FromBody] User user)
    {
      user.Id = Guid.NewGuid();
      _repo.Add(user);
      return CreatedAtRoute("GetUser", new { id = user.Id }, user);
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] User updated)
    {
      var existing = _repo.Get(id);
      if (existing == null) return NotFound();
      updated.Id = id;
      _repo.Update(updated);
      return NoContent();
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
      var existing = _repo.Get(id);
      if (existing == null) return NotFound();
      _repo.Delete(id);
      return NoContent();
    }
  }
}
