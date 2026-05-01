using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string baseUrl, string token = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        Console.WriteLine($"GET: {endpoint}");

        var response = await _httpClient.GetAsync(endpoint);

        await LogResponse(response);

        return response;
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, string jsonBody)
    {
        Console.WriteLine($"POST: {endpoint}");
        Console.WriteLine($"Body: {jsonBody}");

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);

        await LogResponse(response);

        return response;
    }

    public async Task<HttpResponseMessage> PutAsync(string endpoint, string jsonBody)
    {
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(endpoint, content);

        await LogResponse(response);

        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);

        await LogResponse(response);

        return response;
    }

    private async Task LogResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine($"Response: {content}");
    }
}