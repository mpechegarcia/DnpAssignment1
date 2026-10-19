using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase {
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public PostsController(
        IPostRepository postRepository,
        IUserRepository userRepository) {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost(
        [FromBody] CreatePostDto request) {
        try {
            await userRepository.GetSingleAsync(request.UserId);

            Post post = new Post {
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId
            };

            Post created = await postRepository.AddAsync(post);

            PostDto dto = new PostDto {
                Id = created.Id,
                Title = created.Title,
                Body = created.Body,
                UserId = created.UserId
            };

            return Created($"/Posts/{dto.Id}", dto);
        }
        catch (InvalidOperationException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSingle(int id) {
        try {
            Post post = await postRepository.GetSingleAsync(id);

            PostDto dto = new PostDto {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            };

            return Ok(dto);
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? username) {
        IQueryable<Post> posts = postRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(title)) {
            posts = posts.Where(
                post => post.Title.Contains(title)
            );
        }

        if (userId != null) {
            posts = posts.Where(
                post => post.UserId == userId
            );
        }

        if (!string.IsNullOrWhiteSpace(username)) {
            List<int> userIds = userRepository
                .GetMany()
                .Where(user => user.UserName.Contains(username))
                .Select(user => user.Id)
                .ToList();

            posts = posts.Where(
                post => userIds.Contains(post.UserId)
            );
        }

        IEnumerable<PostDto> dtos = posts.Select(post => new PostDto {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        });

        return Ok(dtos);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePost(
        int id,
        [FromBody] CreatePostDto request) {
        try {
            await userRepository.GetSingleAsync(request.UserId);

            Post post = new Post {
                Id = id,
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId
            };

            await postRepository.UpdateAsync(post);

            return NoContent();
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost(int id) {
        try {
            await postRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }
}