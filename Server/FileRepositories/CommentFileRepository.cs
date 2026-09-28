using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository{
    private readonly string filePath = "comments.json";

    public async Task<Comment> AddAsync(Comment comment) {
        List<Comment> comments = await ReadCommentsAsync();

        comment.Id = comments.Any()
            ? comments.Max(c => c.Id) + 1
            : 1;

        comments.Add(comment);

        await WriteCommentsAsync(comments);

        return comment;
    }

    public async Task UpdateAsync(Comment comment) {
        List<Comment> comments = await ReadCommentsAsync();

        Comment? existingComment =
            comments.SingleOrDefault(c => c.Id == comment.Id);

        if (existingComment is null) {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        await WriteCommentsAsync(comments);
    }

    public async Task DeleteAsync(int id) {
        List<Comment> comments = await ReadCommentsAsync();

        Comment? commentToRemove =
            comments.SingleOrDefault(c => c.Id == id);

        if (commentToRemove is null) {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);

        await WriteCommentsAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id) {
        List<Comment> comments = await ReadCommentsAsync();

        Comment? comment =
            comments.SingleOrDefault(c => c.Id == id);

        if (comment is null) {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany() {
        string json = File.Exists(filePath)
            ? File.ReadAllTextAsync(filePath).Result
            : "[]";

        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(json)
            ?? new List<Comment>();

        return comments.AsQueryable();
    }

    private async Task<List<Comment>> ReadCommentsAsync() {
        if (!File.Exists(filePath)) {
            return new List<Comment>();
        }

        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<Comment>>(json)
               ?? new List<Comment>();
    }

    private async Task WriteCommentsAsync(List<Comment> comments) {
        string json = JsonSerializer.Serialize(comments);

        await File.WriteAllTextAsync(filePath, json);
    }
}
