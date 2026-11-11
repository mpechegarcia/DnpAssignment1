using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse =
            await client.PostAsJsonAsync("Posts", request);

        string response =
            await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDto>(
            response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
    }

    public async Task<List<PostDto>> GetPostsAsync()
    {
        return await client.GetFromJsonAsync<List<PostDto>>("Posts")
               ?? new List<PostDto>();
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        return await client.GetFromJsonAsync<PostDto>($"Posts/{id}")
               ?? throw new Exception("Post not found.");
    }
}