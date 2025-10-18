using System.Text;
using System.Text.Json;

namespace Store.Tests;

public class Auth(AspireAppHostFixture fixture) : IClassFixture<AspireAppHostFixture>
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);
        using var response = await httpClient.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenDataIsEmpty()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var newUser = new { email = "", password = "" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenEmailIsInvalid()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = mailId, password = "very-long-password-1" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordIsTooShort()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "sh0rt" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordIsMissingDigit()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "very-long-password-without-digit" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPhoneIsInvalid()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "very-long-password-1", phone = "111111111111" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsConflict_WhenUserAlreadyExists()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "very-long-password-1", phone = "+380999999999" };
        var json = JsonSerializer.Serialize(newUser);

        using var content1 = new StringContent(json, Encoding.UTF8, "application/json");
        using var content2 = new StringContent(json, Encoding.UTF8, "application/json");

        using var response1 = await httpClient.PostAsync("/users", content1);
        using var response2 = await httpClient.PostAsync("/users", content2);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated_WhenDataIsValid()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "very-long-password-1", phone = "+380999999999" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}