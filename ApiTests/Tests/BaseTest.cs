using Microsoft.Extensions.DependencyInjection;

public class BaseTest
{
    protected ApiClient Client;

    [SetUp]
    public void Setup()
    {
        var provider = ServiceProviderFactory.Create();
        Client = provider.GetService<ApiClient>();
    }
}