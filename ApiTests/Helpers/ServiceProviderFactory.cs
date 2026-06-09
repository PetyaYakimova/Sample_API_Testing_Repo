using Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderFactory
{
    public static ServiceProvider Create()
    {
        var services = new ServiceCollection();

        var config = ConfigHelper.GetConfig();
        var baseUrl = config["ApiSettings:BaseUrl"];

        // Register HttpClient with Polly
        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(RetryPolicies.GetRetryPolicy());

        // Typed clients
        services.AddSingleton<UserClient>();
        services.AddSingleton<PostClient>();

        return services.BuildServiceProvider();
    }
}