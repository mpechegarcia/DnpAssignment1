using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcCommentRepository : ICommentRepository {
    private readonly AppContext context;

    public EfcCommentRepository(AppContext context){
        this.context = context;
    }

    public async Task<Comment> AddAsync(Comment comment) {
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();

        return comment;
    }

    public async Task UpdateAsync(Comment comment) {
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id) {
        Comment? comment = await context.Comments.FindAsync(id);

        if (comment == null) {
            throw new Exception($"Comment with id {id} not found.");
        }

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<Comment> GetSingleAsync(int id) {
        Comment? comment = await context.Comments.FindAsync(id);

        if (comment == null) {
            throw new Exception($"Comment with id {id} not found.");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany() {
        return context.Comments.AsQueryable();
    }
}