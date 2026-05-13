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
    public async Task GetUsers_ShouldReturnValidUsers()
    {
        // Act
        var response = await UserClient.GetUsers();

        // Assert status
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Deserialize
        var content = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<User>>(content);

        // Assert data
        users.Should().NotBeNull();
        users.Should().NotBeEmpty();

        users.Should().AllSatisfy(user =>
        {
            user.id.Should().BeGreaterThan(0);

            user.name.Should().NotBeNullOrWhiteSpace();

            user.username.Should().NotBeNullOrWhiteSpace();

            user.email.Should().Contain("@");
        });
    }

    [Test]
    public async Task CreateUser_ShouldReturnCreated()
    {
        var body = new UserBuilder()
        .WithName("John")
        .WithEmail("john@test.com")
        .Build();

        var response = await UserClient.CreateUser(body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task UpdateUser_ShouldReturnOk()
    {
        var jsonBody = @"{ ""name"": ""Updated Name"" }";

        var response = await Client.PutAsync("/users/1", jsonBody);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Test]
    public async Task DeleteUser_ShouldReturnOk()
    {
        var response = await Client.DeleteAsync("/users/1");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}