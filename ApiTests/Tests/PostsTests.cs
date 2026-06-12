using FluentAssertions;
using Newtonsoft.Json;
using System.Net;

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
}
