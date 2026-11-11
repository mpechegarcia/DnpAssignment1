using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage httpResponse =
            await client.PostAsJsonAsync("Comments", request);

        string response =
            await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDto>(
            response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
    }

    public async Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId)
    {
        return await client.GetFromJsonAsync<List<CommentDto>>(
                   $"Comments?postId={postId}")
               ?? new List<CommentDto>();
    }
}