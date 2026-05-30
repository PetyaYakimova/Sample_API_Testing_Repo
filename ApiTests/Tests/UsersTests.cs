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

        // Assert status and content type
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

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
    public async Task GetUsers_WithInvalidQueryParameter_ShouldHandleRequest()
    {
        // Act
        var response = await Client.GetAsync("/users?invalidParam=test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetUser_ShouldReturnCorrectUser()
    {
        // Arrange
        int userId = 1;

        // Act
        var response = await UserClient.GetUser(userId);

        // Assert status and content type
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();

        var user = JsonConvert.DeserializeObject<User>(content);

        // Validate exact returned data
        user.Should().NotBeNull();

        user.id.Should().Be(userId);

        user.name.Should().NotBeNullOrWhiteSpace();

        user.email.Should().Contain("@");
    }

    [Test]
    public async Task GetUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        int invalidId = 99999;

        // Act
        var response = await UserClient.GetUser(invalidId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetUser_WithNegativeId_ShouldReturnNotFound()
    {
        // Arrange
        int invalidId = -1;

        // Act
        var response = await UserClient.GetUser(invalidId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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

        // Assert status and content type
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

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
    public async Task CreateUser_WithEmptyBody_ShouldHandleRequest()
    {
        // Arrange
        string emptyBody = "";

        // Act
        var response = await Client.PostAsync("/users", emptyBody);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithMalformedJson_ShouldFailGracefully()
    {
        // Arrange
        string invalidJson = "{ name: John ";

        // Act
        var response = await Client.PostAsync("/users", invalidJson);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithInvalidEmailType_ShouldHandleValidation()
    {
        // Arrange
        string invalidBody = @"
        {
            ""name"": ""John"",
            ""username"": ""john123"",
            ""email"": 12345
        }";

        // Act
        var response = await Client.PostAsync("/users", invalidBody);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithoutEmail_ShouldHandleValidation()
    {
        // Arrange
        string invalidBody = @"
        {
            ""name"": ""John"",
            ""username"": ""john123""
        }";

        // Act
        var response = await Client.PostAsync("/users", invalidBody);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithVeryLongName_ShouldHandleRequest()
    {
        // Arrange
        string longName = new string('A', 10000);

        string body = $@"
        {{
            ""name"": ""{longName}"",
            ""username"": ""test"",
            ""email"": ""test@test.com""
        }}";

        // Act
        var response = await Client.PostAsync("/users", body);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithNullValues_ShouldHandleRequest()
    {
        // Arrange
        string body = @"
        {
            ""name"": null,
            ""username"": null,
            ""email"": null
        }";

        // Act
        var response = await Client.PostAsync("/users", body);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithSqlInjectionInput_ShouldHandleSafely()
    {
        // Arrange
        string body = @"
        {
            ""name"": ""' OR 1=1 --"",
            ""username"": ""hacker"",
            ""email"": ""hack@test.com""
        }";

        // Act
        var response = await Client.PostAsync("/users", body);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task CreateUser_WithXssPayload_ShouldHandleSafely()
    {
        // Arrange
        string body = @"
        {
            ""name"": ""<script>alert('xss')</script>"",
            ""username"": ""test"",
            ""email"": ""xss@test.com""
        }";

        // Act
        var response = await Client.PostAsync("/users", body);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task UpdateUser_ShouldReturnUpdatedUser()
    {
        // Arrange
        int userId = 1;

        var updatedRequest = new
        {
            id = userId,
            name = "Updated User",
            username = "updateduser",
            email = "updated@test.com"
        };

        var json = JsonConvert.SerializeObject(updatedRequest);

        // Act
        var response = await UserClient.UpdateUser(userId, json);

        // Assert status and content type
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();

        var updatedUser = JsonConvert.DeserializeObject<User>(content);

        // Validate updated values
        updatedUser.Should().NotBeNull();

        updatedUser.id.Should().Be(userId);

        updatedUser.name.Should().Be(updatedRequest.name);

        updatedUser.username.Should().Be(updatedRequest.username);

        updatedUser.email.Should().Be(updatedRequest.email);

        //Follow up assertion with getting the user cannot be made, since the API is fake and does not persist data.
    }

    [Test]
    public async Task PatchUser_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        int userId = 1;

        var patchRequest = new
        {
            name = "Patched Name"
        };

        var json = JsonConvert.SerializeObject(patchRequest);

        // Act
        var response = await UserClient.PatchUser(userId, json);

        // Assert status
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Deserialize response
        var content = await response.Content.ReadAsStringAsync();

        var patchedUser = JsonConvert.DeserializeObject<User>(content);

        // Validate patched field
        patchedUser.Should().NotBeNull();

        patchedUser.name.Should().Be("Patched Name");

        // Validate untouched fields still exist
        patchedUser.id.Should().Be(userId);

        //Follow up assertion with getting the user cannot be made, since the API is fake and does not persist data.
    }

    [Test]
    public async Task DeleteUser_ShouldReturnEmptyResponse()
    {
        // Arrange
        int userId = 1;

        // Act
        var response = await UserClient.DeleteUser(userId);

        // Assert status and content type
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Be("{}");

        //Follow up assertion with trying to get the user cannot be made, since the API is fake and does not persist data.
    }

    [Test]
    public async Task Request_ShouldTimeout_WhenTimeoutTooSmall()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
            Timeout = TimeSpan.FromMilliseconds(1)
        };

        // Act
        Func<Task> act = async () =>
        {
            await httpClient.GetAsync("/users");
        };

        // Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
    }
}