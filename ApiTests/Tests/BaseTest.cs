using NUnit.Framework;

public class BaseTest
{
    protected ApiClient Client;

    [SetUp]
    public void BaseSetup()
    {
        var config = ConfigHelper.GetConfig();
        var baseUrl = config["ApiSettings:BaseUrl"];

        Client = new ApiClient(baseUrl);
    }
}