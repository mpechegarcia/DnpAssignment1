using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase {
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository) {
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request) {
        User user = new User {
            UserName = request.UserName,
            Password = request.Password
        };

        User created = await userRepository.AddAsync(user);

        UserDto dto = new UserDto {
            Id = created.Id,
            UserName = created.UserName
        };

        return Created($"/Users/{dto.Id}", dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id) {
        try {
            User user = await userRepository.GetSingleAsync(id);

            UserDto dto = new UserDto {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(dto);
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany(
        [FromQuery] string? username) {
        IQueryable<User> users = userRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(username)) {
            users = users.Where(
                user => user.UserName.Contains(username)
            );
        }

        IEnumerable<UserDto> dtos = users.Select(user => new UserDto {
            Id = user.Id,
            UserName = user.UserName
        });

        return Ok(dtos);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser(
        int id,
        [FromBody] CreateUserDto request) {
        try {
            User user = new User {
                Id = id,
                UserName = request.UserName,
                Password = request.Password
            };

            await userRepository.UpdateAsync(user);

            return NoContent();
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser(int id) {
        try {
            await userRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }
}