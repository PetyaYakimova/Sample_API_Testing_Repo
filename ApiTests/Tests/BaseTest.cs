using Microsoft.Extensions.DependencyInjection;

public class BaseTest
{
    protected ApiClient Client;
    protected UserClient UserClient;

    [SetUp]
    public void Setup()
    {
        var provider = ServiceProviderFactory.Create();

        Client = provider.GetService<ApiClient>();
        UserClient = provider.GetService<UserClient>();
    }
}