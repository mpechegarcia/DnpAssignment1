using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase {
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository) {
        this.userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(
        [FromBody] LoginRequest request) {
        User? user = await userRepository
            .GetMany()
            .SingleOrDefaultAsync(
                user => user.UserName == request.UserName
            );

        if (user == null) {
            return Unauthorized("User not found.");
        }

        if (user.Password != request.Password) {
            return Unauthorized("Incorrect password.");
        }

        UserDto dto = new UserDto {
            Id = user.Id,
            UserName = user.UserName
        };

        return Ok(dto);
    }
}