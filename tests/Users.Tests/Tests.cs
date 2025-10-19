using System.Text;
using System.Text.Json;

namespace Users.Tests;

public class Tests(AspireAppHostFixture fixture) : IClassFixture<AspireAppHostFixture>
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);

    private const string ValidPassword = "VeryLongPassword123";

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenDataIsEmpty()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var newUser = new { email = "", password = "" };
        var json = JsonSerializer.Serialize(newUser);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenEmailIsInvalid()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var newUser = new { email = Guid.NewGuid(), password = ValidPassword };
        var json = JsonSerializer.Serialize(newUser);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordIsTooShort()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var email = $"{Guid.NewGuid()}@example.com";

        var newUser = new { email, password = "pass1" };
        var json = JsonSerializer.Serialize(newUser);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordIsMissingDigit()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var email = $"{Guid.NewGuid()}@example.com";

        var newUser = new { email, password = "VeryLongPassword" };
        var json = JsonSerializer.Serialize(newUser);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPhoneIsInvalid()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var email = $"{Guid.NewGuid()}@example.com";

        var newUser = new { email, password = ValidPassword, phone = "11111111111111" };
        var json = JsonSerializer.Serialize(newUser);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_Conflicts_WhenEmailIsAlreadyTaken()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var email = $"{Guid.NewGuid()}@example.com";

        var newUser = new { email, password = ValidPassword };
        var json = JsonSerializer.Serialize(newUser);

        using var content1 = new StringContent(json, Encoding.UTF8, "application/json");
        using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
        using var response1 = await httpClient.PostAsync("/", content1);
        using var response2 = await httpClient.PostAsync("/", content2);

        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated_WhenDataIsValid()
    {
        using var httpClient = fixture.App.CreateHttpClient("UsersAPI");
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var email = $"{Guid.NewGuid()}@example.com";

        var newUser = new { email, password = ValidPassword };
        var json = JsonSerializer.Serialize(newUser);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}