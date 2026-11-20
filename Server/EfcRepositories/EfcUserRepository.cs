using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcUserRepository : IUserRepository {
    private readonly AppContext context;

    public EfcUserRepository(AppContext context) {
        this.context = context;
    }

    public async Task<User> AddAsync(User user) {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task UpdateAsync(User user) {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id) {
        User? user = await context.Users.FindAsync(id);

        if (user == null) {
            throw new Exception($"User with id {id} not found.");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetSingleAsync(int id) {
        User? user = await context.Users.FindAsync(id);

        if (user == null) {
            throw new Exception($"User with id {id} not found.");
        }

        return user;
    }

    public IQueryable<User> GetMany() {
        return context.Users.AsQueryable();
    }
}