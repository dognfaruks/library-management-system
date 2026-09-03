using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LibraryManagementSystem.Application.DTOs.Auth;
using LibraryManagementSystem.Application.DTOs.Borrowing;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests;

public class BorrowingEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public BorrowingEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    /// Yardımcı metod: yeni bir kullanıcı kaydedip token döner
    private async Task<string> RegisterAndGetTokenAsync(string email)
    {
        var request = new RegisterRequest
        {
            Username = "borrowuser_" + Guid.NewGuid().ToString("N")[..8],
            Email = email,
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/auth/register", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return result!.Token;
    }

    /// Yardımcı metod: test için doğrudan veritabanına bir kitap ekler (Stock ile)
    private Guid SeedBookWithStock(int stock)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

        var publisher = new Publisher { Id = Guid.NewGuid(), Name = "TestPublisher_" + Guid.NewGuid() };
        db.Publishers.Add(publisher);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Kitap",
            ISBN = Guid.NewGuid().ToString("N")[..13],
            PublisherId = publisher.Id,
            Stock = stock
        };
        db.Books.Add(book);
        db.SaveChanges();

        return book.Id;
    }

    

    [Fact]
    public async Task Create_TokenOlmadan_401Doner()
    {
        // Arrange: Authorization header eklemiyoruz
        var bookId = SeedBookWithStock(stock: 5);

        // Act
        var response = await _client.PostAsJsonAsync("/borrowings", new CreateBorrowingRequest { BookId = bookId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}