using System;
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
            BaseAddress = new Uri(baseUrl)
        };
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        Console.WriteLine($"GET: {endpoint}");

        var response = await _httpClient.GetAsync(endpoint);

        Console.WriteLine($"Status: {response.StatusCode}");

        return response;
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, string jsonBody)
    {
        Console.WriteLine($"POST: {endpoint}");
        Console.WriteLine($"Body: {jsonBody}");

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);

        Console.WriteLine($"Status: {response.StatusCode}");

        return response;
    }
}