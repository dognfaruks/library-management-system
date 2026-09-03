using System.Net;
using System.Net.Http.Json;
using LibraryManagementSystem.Application.DTOs.Auth;
using FluentAssertions;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_GecerliBilgilerle_200veTokenDoner()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "integrationuser",
            Email = "integration@test.com",
            Password = "Password123!"
        };

        // Act: gerçek bir HTTP POST isteği gönderiyoruz
        var response = await _client.PostAsJsonAsync("/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.Username.Should().Be("integrationuser");
        result.Token.Should().NotBeNullOrEmpty();
        result.Roles.Should().Contain("USER");
    }

    [Fact]
    public async Task Register_AyniEmailIleTekrar_409Doner()
    {
        // Arrange: aynı email ile ilk kaydı yapıyoruz
        var request = new RegisterRequest
        {
            Username = "user1",
            Email = "duplicate@test.com",
            Password = "Password123!"
        };
        await _client.PostAsJsonAsync("/auth/register", request);

        // Act: aynı email ile tekrar kayıt olmaya çalışıyoruz
        var secondRequest = new RegisterRequest
        {
            Username = "user2",
            Email = "duplicate@test.com", // aynı email
            Password = "Password456!"
        };
        var response = await _client.PostAsJsonAsync("/auth/register", secondRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_YanlisSifreIle_401Doner()
    {
        // Arrange: önce bir kullanıcı oluştur
        var registerRequest = new RegisterRequest
        {
            Username = "loginuser",
            Email = "login@test.com",
            Password = "CorrectPassword123!"
        };
        await _client.PostAsJsonAsync("/auth/register", registerRequest);

        // Act: yanlış şifreyle giriş yapmaya çalış
        var loginRequest = new LoginRequest
        {
            Email = "login@test.com",
            Password = "WrongPassword!"
        };
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_TokenOlmadan_401Doner()
    {
        // Act: token vermeden korumalı endpoint'e istek at
        var response = await _client.GetAsync("/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}