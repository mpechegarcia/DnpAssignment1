using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase {
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public CommentsController(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IPostRepository postRepository) {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment(
        [FromBody] CreateCommentDto request) {
        try {
            await userRepository.GetSingleAsync(request.UserId);
            await postRepository.GetSingleAsync(request.PostId);

            Comment comment = new Comment {
                Body = request.Body,
                UserId = request.UserId,
                PostId = request.PostId
            };

            Comment created = await commentRepository.AddAsync(comment);

            CommentDto dto = new CommentDto {
                Id = created.Id,
                Body = created.Body,
                UserId = created.UserId,
                PostId = created.PostId
            };

            return Created($"/Comments/{dto.Id}", dto);
        }
        catch (InvalidOperationException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingle(int id) {
        try {
            Comment comment = await commentRepository.GetSingleAsync(id);

            CommentDto dto = new CommentDto {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                PostId = comment.PostId
            };

            return Ok(dto);
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany(
        [FromQuery] int? userId,
        [FromQuery] string? username,
        [FromQuery] int? postId) {
        IQueryable<Comment> comments = commentRepository.GetMany();

        if (userId != null) {
            comments = comments.Where(
                comment => comment.UserId == userId
            );
        }

        if (!string.IsNullOrWhiteSpace(username)) {
            List<int> userIds = userRepository
                .GetMany()
                .Where(user => user.UserName.Contains(username))
                .Select(user => user.Id)
                .ToList();

            comments = comments.Where(
                comment => userIds.Contains(comment.UserId)
            );
        }

        if (postId != null) {
            comments = comments.Where(
                comment => comment.PostId == postId
            );
        }

        IEnumerable<CommentDto> dtos =
            comments.Select(comment => new CommentDto {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                PostId = comment.PostId
            });

        return Ok(dtos);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateComment(
        int id,
        [FromBody] CreateCommentDto request) {
        try {
            await userRepository.GetSingleAsync(request.UserId);
            await postRepository.GetSingleAsync(request.PostId);

            Comment comment = new Comment {
                Id = id,
                Body = request.Body,
                UserId = request.UserId,
                PostId = request.PostId
            };

            await commentRepository.UpdateAsync(comment);

            return NoContent();
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment(int id) {
        try {
            await commentRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException e) {
            return NotFound(e.Message);
        }
    }
}