using NUnit.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[TestFixture]
public class UsersTests : BaseTest
{
    [Test]
    public async Task GetUsers_ShouldReturnUsersList()
    {
        // Act
        var response = await Client.GetAsync("/users");

        // Assert status
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<User>>(content);

        users.Should().NotBeNull();
        users.Count.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task CreateUser_ShouldReturnCreated()
    {
        var jsonBody = @"{
        ""name"": ""John Doe"",
        ""username"": ""johndoe"",
        ""email"": ""john@test.com""
    }";

        var response = await Client.PostAsync("/users", jsonBody);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}