using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository {
    private readonly string filePath = "users.json";

    public async Task<User> AddAsync(User user) {
        List<User> users = await ReadUsersAsync();

        user.Id = users.Any()
            ? users.Max(u => u.Id) + 1
            : 1;

        users.Add(user);

        await WriteUsersAsync(users);

        return user;
    }

    public async Task UpdateAsync(User user) {
        List<User> users = await ReadUsersAsync();

        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);

        if (existingUser is null) {
            throw new InvalidOperationException(
                $"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        await WriteUsersAsync(users);
    }

    public async Task DeleteAsync(int id) {
        List<User> users = await ReadUsersAsync();

        User? userToRemove = users.SingleOrDefault(u => u.Id == id);

        if (userToRemove is null) {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);

        await WriteUsersAsync(users);
    }

    public async Task<User> GetSingleAsync(int id) {
        List<User> users = await ReadUsersAsync();

        User? user = users.SingleOrDefault(u => u.Id == id);

        if (user is null) {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        return user;
    }

    public IQueryable<User> GetMany() {
        string json = File.Exists(filePath)
            ? File.ReadAllTextAsync(filePath).Result
            : "[]";

        List<User> users =
            JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

        return users.AsQueryable();
    }

    private async Task<List<User>> ReadUsersAsync() {
        if (!File.Exists(filePath)) {
            return new List<User>();
        }

        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<User>>(json)
               ?? new List<User>();
    }

    private async Task WriteUsersAsync(List<User> users) {
        string json = JsonSerializer.Serialize(users);

        await File.WriteAllTextAsync(filePath, json);
    }
}