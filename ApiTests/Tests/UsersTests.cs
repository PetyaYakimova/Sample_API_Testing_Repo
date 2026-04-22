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
        var users = ResponseHelper.Deserialize<List<User>>(content);

        users.Should().NotBeNull();
        users.Count.Should().BeGreaterThan(0);
        users.Should().AllSatisfy(u =>
        {
            u.id.Should().BeGreaterThan(0);
            u.email.Should().Contain("@");
        });
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