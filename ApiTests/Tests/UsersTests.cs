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
    public async Task GetUser_ShouldReturnCorrectUser()
    {
        // Arrange
        int userId = 1;

        // Act
        var response = await UserClient.GetUser(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var user = JsonConvert.DeserializeObject<User>(content);

        // Validate exact returned data
        user.Should().NotBeNull();

        user.id.Should().Be(userId);

        user.name.Should().NotBeNullOrWhiteSpace();

        user.email.Should().Contain("@");
    }

    [Test]
    public async Task CreateUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var requestBody = new
        {
            name = "John Doe",
            username = "johndoe",
            email = "john@test.com"
        };

        var json = JsonConvert.SerializeObject(requestBody);

        // Act
        var response = await UserClient.CreateUser(json);

        // Assert status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Deserialize response
        var content = await response.Content.ReadAsStringAsync();

        var createdUser = JsonConvert.DeserializeObject<User>(content);

        // Validate returned values
        createdUser.Should().NotBeNull();

        createdUser.id.Should().BeGreaterThan(0);

        createdUser.name.Should().Be(requestBody.name);

        createdUser.username.Should().Be(requestBody.username);

        createdUser.email.Should().Be(requestBody.email);

        //Follow up assertion with getting the user cannot be made, since the API is fake and does not persist data.
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