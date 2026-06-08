public class PostClient
{
    private readonly ApiClient _apiClient;

    public PostClient(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<HttpResponseMessage> GetPosts()
    {
        return await _apiClient.GetAsync("/posts");
    }

    public async Task<HttpResponseMessage> GetPost(int id)
    {
        return await _apiClient.GetAsync($"/posts/{id}");
    }

    public async Task<HttpResponseMessage> GetPostsByUser(int userId)
    {
        return await _apiClient.GetAsync($"/posts?userId={userId}");
    }

    public async Task<HttpResponseMessage> CreatePost(string body)
    {
        return await _apiClient.PostAsync("/posts", body);
    }

    public async Task<HttpResponseMessage> UpdatePost(int id, string body)
    {
        return await _apiClient.PutAsync($"/posts/{id}", body);
    }

    public async Task<HttpResponseMessage> PatchPost(int id, string body)
    {
        return await _apiClient.PatchAsync($"/posts/{id}", body);
    }

    public async Task<HttpResponseMessage> DeletePost(int id)
    {
        return await _apiClient.DeleteAsync($"/posts/{id}");
    }
}