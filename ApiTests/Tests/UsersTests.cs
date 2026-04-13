using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[TestFixture]
public class UsersTests
{
    private ApiClient _client;

    [SetUp]
    public void Setup()
    {
        _client = new ApiClient("https://jsonplaceholder.typicode.com");
    }

    [Test]
    public async Task GetUsers_ShouldReturnUsersList()
    {
        // Act
        var response = await _client.GetAsync("/users");

        // Assert status
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<User>>(content);

        users.Should().NotBeNull();
        users.Count.Should().BeGreaterThan(0);
    }
}