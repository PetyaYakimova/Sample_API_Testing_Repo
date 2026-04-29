using System.Net.Http;
using System.Threading.Tasks;

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
}