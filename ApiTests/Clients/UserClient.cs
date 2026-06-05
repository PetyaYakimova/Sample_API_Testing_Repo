public class UserClient
{
    private readonly ApiClient _apiClient;

    public UserClient(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<HttpResponseMessage> GetUsers()
    {
        return await _apiClient.GetAsync("/users");
    }

    public async Task<HttpResponseMessage> GetUser(int id)
    {
        return await _apiClient.GetAsync($"/users/{id}");
    }

    public async Task<HttpResponseMessage> CreateUser(string body)
    {
        return await _apiClient.PostAsync("/users", body);
    }

    public async Task<HttpResponseMessage> UpdateUser(int id, string body)
    {
        return await _apiClient.PutAsync($"/users/{id}", body);
    }

    public async Task<HttpResponseMessage> PatchUser(int id, string body)
    {
        return await _apiClient.PatchAsync($"/users/{id}", body);
    }

    public async Task<HttpResponseMessage> DeleteUser(int id)
    {
        return await _apiClient.DeleteAsync($"/users/{id}");
    }
}