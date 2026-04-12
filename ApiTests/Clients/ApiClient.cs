using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new System.Uri(baseUrl)
        };
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        return await _httpClient.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, string jsonBody)
    {
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(endpoint, content);
    }
}