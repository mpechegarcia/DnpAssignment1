using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcPostRepository : IPostRepository {
    private readonly AppContext context;

    public EfcPostRepository(AppContext context) {
        this.context = context;
    }

    public async Task<Post> AddAsync(Post post) {
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();

        return post;
    }

    public async Task UpdateAsync(Post post) {
        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id) {
        Post? post = await context.Posts.FindAsync(id);

        if (post == null) {
            throw new Exception($"Post with id {id} not found.");
        }

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<Post> GetSingleAsync(int id) {
        Post? post = await context.Posts.FindAsync(id);

        if (post == null) {
            throw new Exception($"Post with id {id} not found.");
        }

        return post;
    }

    public IQueryable<Post> GetMany() {
        return context.Posts.AsQueryable();
    }
}