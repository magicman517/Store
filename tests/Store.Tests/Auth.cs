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

        var newUser = new { email = mailId, password = "ValidPassword123!" };
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

        var newUser = new { email = $"{mailId}@example.com", password = "Sh0rt!" };
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

        var newUser = new { email = $"{mailId}@example.com", password = "NoDigitsHere!" };
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

        var newUser = new { email = $"{mailId}@example.com", password = "ValidPassword123!", phone = "111111111111" };
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

        var newUser = new { email = $"{mailId}@example.com", password = "ValidPassword123!", phone = "+380999999999" };
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
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordMissingUppercase()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "lowercase1!" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordMissingLowercase()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "UPPERCASE1!" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenPasswordMissingSpecialChar()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "Password123" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated_WhenDataIsValid()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var newUser = new { email = $"{mailId}@example.com", password = "ValidPassword123!", phone = "+380999999999" };
        var json = JsonSerializer.Serialize(newUser);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenCredentialsAreEmpty()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var loginRequest = new { email = "", password = "" };
        var json = JsonSerializer.Serialize(loginRequest);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/auth/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();

        var loginRequest = new { email = $"{mailId}@example.com", password = "ValidPassword123!" };
        var json = JsonSerializer.Serialize(loginRequest);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/auth/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        // Arrange
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var mailId = Guid.NewGuid().ToString();
        var password = "ValidPassword123!";

        var newUser = new { email = $"{mailId}@example.com", password, phone = "+380999999999" };
        var json = JsonSerializer.Serialize(newUser);
        using var createContent = new StringContent(json, Encoding.UTF8, "application/json");
        using var createResponse = await httpClient.PostAsync("/users", createContent);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act
        var loginRequest = new { email = $"{mailId}@example.com", password };
        var loginJson = JsonSerializer.Serialize(loginRequest);
        using var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");
        using var loginResponse = await httpClient.PostAsync("/auth/login", loginContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var responseBody = await loginResponse.Content.ReadAsStringAsync();
        Assert.Contains("accessToken", responseBody);
        Assert.Contains("refreshToken", responseBody);
    }

    [Fact]
    public async Task Refresh_ReturnsBadRequest_WhenTokenIsEmpty()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var refreshRequest = new { refreshToken = "" };
        var json = JsonSerializer.Serialize(refreshRequest);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/auth/refresh", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ReturnsBadRequest_WhenTokenIsInvalid()
    {
        // Act
        using var httpClient = fixture.App!.CreateHttpClient("AuthAPI");
        await fixture.App!.ResourceNotifications.WaitForResourceHealthyAsync("AuthAPI").WaitAsync(DefaultTimeout);

        var refreshRequest = new { refreshToken = "invalid-token" };
        var json = JsonSerializer.Serialize(refreshRequest);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/auth/refresh", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}