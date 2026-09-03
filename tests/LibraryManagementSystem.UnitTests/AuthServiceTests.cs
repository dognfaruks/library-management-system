using LibraryManagementSystem.Application.DTOs.Auth;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Persistence;
using LibraryManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;

namespace LibraryManagementSystem.UnitTests;

public class AuthServiceTests
{
    private readonly LibraryDbContext _context;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Her test için TAMAMEN İZOLE, benzersiz isimli bir InMemory veritabanı oluşturuyoruz
        // (Guid.NewGuid() sayesinde testler birbirinin verisini görmez/etkilemez)
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new LibraryDbContext(options);

        // USER rolünü elle ekliyoruz (gerçek veritabanında bunu migration seed data yapıyordu,
        // InMemory'de migration çalışmadığı için elle eklememiz gerekiyor)
        _context.Roles.Add(new Role { Id = 1, Name = "USER" });
        _context.SaveChanges();

        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        var loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _context,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_YeniEmailIle_BasariylaKullaniciOlusturur()
    {
        // Arrange (hazırlık): sahte davranışları tanımlıyoruz
        _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");
        _jwtTokenGeneratorMock
            .Setup(j => j.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
            .Returns(("fake_token", DateTime.UtcNow.AddHours(1)));

        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act (asıl test edilen eylem)
        var result = await _authService.RegisterAsync(request);

        // Assert (doğrulama)
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Roles.Should().Contain("USER");
        result.Token.Should().Be("fake_token");
    }

    [Fact]
    public async Task RegisterAsync_VarOlanEmailIle_DuplicateEmailExceptionFirlatir()
    {
        // Arrange: veritabanına önceden bir kullanıcı ekliyoruz
        _context.Users.Add(new User
        {
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = "some_hash"
        });
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "existing@example.com", // aynı email
            Password = "Password123!"
        };

        // Act & Assert: bu işlemin DuplicateEmailException fırlatmasını bekliyoruz
        var act = async () => await _authService.RegisterAsync(request);
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }

    [Fact]
    public async Task LoginAsync_YanlisSifreIle_InvalidCredentialsExceptionFirlatir()
    {
        // Arrange
        _context.Users.Add(new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "correct_hash"
        });
        await _context.SaveChangesAsync();

        // Şifre doğrulamasının HER ZAMAN false dönmesini sağlıyoruz (yanlış şifre senaryosu)
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrong_password"
        };

        // Act & Assert
        var act = async () => await _authService.LoginAsync(request);
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}