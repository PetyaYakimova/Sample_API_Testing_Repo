using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;

[TestFixture]

public class PostsTests : BaseTest
{
    [Test]
    public async Task GetPosts_ShouldReturnValidPosts()
    {
        var response = await PostClient.GetPosts();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var posts = JsonConvert.DeserializeObject<List<Post>>(content);

        posts.Should().NotBeNull();
        posts.Should().NotBeEmpty();

        posts.Should().AllSatisfy(post =>
        {
            post.Id.Should().BeGreaterThan(0);

            post.UserId.Should().BeGreaterThan(0);

            post.Title.Should().NotBeNullOrWhiteSpace();

            post.Body.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Test]
    public async Task GetPost_ShouldReturnCorrectPost()
    {
        int postId = 1;

        var response = await PostClient.GetPost(postId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var post = JsonConvert.DeserializeObject<Post>(content);

        post.Should().NotBeNull();

        post.Id.Should().Be(postId);

        post.UserId.Should().BeGreaterThan(0);

        post.Title.Should().NotBeNullOrWhiteSpace();

        post.Body.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task GetPost_ShouldContainExpectedFields()
    {
        var response = await PostClient.GetPost(1);

        var content = await response.Content.ReadAsStringAsync();

        var post = JObject.Parse(content);

        post["userId"].Should().NotBeNull();
        post["id"].Should().NotBeNull();
        post["title"].Should().NotBeNull();
        post["body"].Should().NotBeNull();
    }

    [Test]
    public async Task GetPost_ShouldReturnJsonContentType()
    {
        var response = await PostClient.GetPost(1);

        response.Content.Headers.ContentType.MediaType
            .Should()
            .Be("application/json");
    }

    [Test]
    public async Task GetPost_ShouldRespondWithinTwoSeconds()
    {
        var stopwatch = Stopwatch.StartNew();

        await PostClient.GetPost(1);

        stopwatch.Stop();

        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);
    }

    [Test]
    public async Task PostOwner_ShouldExist()
    {
        var postResponse = await PostClient.GetPost(1);

        var postContent = await postResponse.Content.ReadAsStringAsync();

        var post = JsonConvert.DeserializeObject<Post>(postContent);

        var userResponse = await UserClient.GetUser(post.UserId);

        userResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetPost_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await PostClient.GetPost(99999);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetPostsByUser_ShouldReturnOnlyRequestedUserPosts()
    {
        int userId = 1;

        var response = await PostClient.GetPostsByUser(userId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var posts = JsonConvert.DeserializeObject<List<Post>>(content);

        posts.Should().NotBeEmpty();

        posts.Should().OnlyContain(p => p.UserId == userId);
    }

    [Test]
    public async Task GetPostsByNonExistingUser_ShouldReturnEmptyCollection()
    {
        var response = await PostClient.GetPostsByUser(99999);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var posts = JsonConvert.DeserializeObject<List<Post>>(content);

        posts.Should().BeEmpty();
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(99999)]
    public async Task GetPostWithInvalidPostIds_ShouldReturnNotFound(int id)
    {
        var response = await PostClient.GetPost(id);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AllPosts_ShouldHaveUniqueIds()
    {
        var response = await PostClient.GetPosts();

        var content = await response.Content.ReadAsStringAsync();

        var posts = JsonConvert.DeserializeObject<List<Post>>(content);

        posts.Select(p => p.Id)
             .Should()
             .OnlyHaveUniqueItems();
    }

    [Test]
    public async Task CreatePost_ShouldReturnCreatedPost()
    {
        var request = new
        {
            userId = 1,
            title = "My New Post",
            body = "Post Content"
        };

        var json = JsonConvert.SerializeObject(request);

        var response = await PostClient.CreatePost(json);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();

        var createdPost = JsonConvert.DeserializeObject<Post>(content);

        createdPost.Should().NotBeNull();

        createdPost.Id.Should().BeGreaterThan(0);

        createdPost.UserId.Should().Be(request.userId);

        createdPost.Title.Should().Be(request.title);

        createdPost.Body.Should().Be(request.body);
    }

    [Test]
    public async Task UpdatePost_ShouldReturnUpdatedPost()
    {
        int postId = 1;

        var request = new
        {
            id = postId,
            userId = 1,
            title = "Updated Title",
            body = "Updated Body"
        };

        var json = JsonConvert.SerializeObject(request);

        var response = await PostClient.UpdatePost(postId, json);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var post = JsonConvert.DeserializeObject<Post>(content);

        post.Title.Should().Be(request.title);

        post.Body.Should().Be(request.body);
    }

    [Test]
    public async Task PatchPost_ShouldUpdateTitleOnly()
    {
        int postId = 1;

        var request = new
        {
            title = "Patched Title"
        };

        var json = JsonConvert.SerializeObject(request);

        var response = await PostClient.PatchPost(postId, json);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var post = JsonConvert.DeserializeObject<Post>(content);

        post.Title.Should().Be("Patched Title");

        post.Body.Should().NotBeNullOrWhiteSpace();

        post.Id.Should().Be(postId);
    }

    [Test]
    public async Task PatchPost_WithInvalidId_ShouldReturnNotFound()
    {
        var json = JsonConvert.SerializeObject(new
        {
            title = "Invalid"
        });

        var response = await PostClient.PatchPost(99999, json);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeletePost_ShouldReturnEmptyObject()
    {
        int postId = 1;

        var response = await PostClient.DeletePost(postId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Be("{}");
    }


}
