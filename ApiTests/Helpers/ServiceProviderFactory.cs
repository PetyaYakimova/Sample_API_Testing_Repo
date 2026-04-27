using Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderFactory
{
    public static ServiceProvider Create()
    {
        var services = new ServiceCollection();

        var config = ConfigHelper.GetConfig();
        var baseUrl = config["ApiSettings:BaseUrl"];

        services.AddSingleton(new ApiClient(baseUrl));

        return services.BuildServiceProvider();
    }
}