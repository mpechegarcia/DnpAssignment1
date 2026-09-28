using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository {
    private readonly string filePath = "posts.json";

    public async Task<Post> AddAsync(Post post) {
        List<Post> posts = await ReadPostsAsync();

        post.Id = posts.Any()
            ? posts.Max(p => p.Id) + 1
            : 1;

        posts.Add(post);

        await WritePostsAsync(posts);

        return post;
    }

    public async Task UpdateAsync(Post post) {
        List<Post> posts = await ReadPostsAsync();

        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);

        if (existingPost is null) {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        await WritePostsAsync(posts);
    }

    public async Task DeleteAsync(int id) {
        List<Post> posts = await ReadPostsAsync();

        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);

        if (postToRemove is null) {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);

        await WritePostsAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id) {
        List<Post> posts = await ReadPostsAsync();

        Post? post = posts.SingleOrDefault(p => p.Id == id);

        if (post is null) {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        return post;
    }

    public IQueryable<Post> GetMany() {
        string json = File.Exists(filePath)
            ? File.ReadAllTextAsync(filePath).Result
            : "[]";

        List<Post> posts =
            JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();

        return posts.AsQueryable();
    }

    private async Task<List<Post>> ReadPostsAsync() {
        if (!File.Exists(filePath)) {
            return new List<Post>();
        }

        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<Post>>(json)
               ?? new List<Post>();
    }

    private async Task WritePostsAsync(List<Post> posts) {
        string json = JsonSerializer.Serialize(posts);

        await File.WriteAllTextAsync(filePath, json);
    }
}