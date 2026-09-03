using LibraryManagementSystem.Application.DTOs.Borrowing;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Persistence;
using LibraryManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;

namespace LibraryManagementSystem.UnitTests;

public class BorrowingServiceTests
{
    private readonly LibraryDbContext _context;
    private readonly BorrowingService _borrowingService;

    public BorrowingServiceTests()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new LibraryDbContext(options);

        var loggerMock = new Mock<ILogger<BorrowingService>>();
        _borrowingService = new BorrowingService(_context, loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_VarOlmayanKitapIcin_NotFoundExceptionFirlatir()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateBorrowingRequest { BookId = Guid.NewGuid() }; // hiç var olmayan bir ID

        // Act & Assert
        var act = async () => await _borrowingService.CreateAsync(userId, request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReturnAsync_VarOlmayanBorrowingIcin_NotFoundExceptionFirlatir()
    {
        // Act & Assert
        var act = async () => await _borrowingService.ReturnAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReturnAsync_ZatenIadeEdilmisBorrowingIcin_AlreadyReturnedExceptionFirlatir()
    {
        // Arrange: durumu "Returned" olan bir borrowing kaydı elle ekliyoruz
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

                var publisher = new Publisher { Id = Guid.NewGuid(), Name = "TestPub" };

        _context.Users.Add(new User { Id = userId, Username = "test", Email = "test@test.com", PasswordHash = "hash" });
        _context.Publishers.Add(publisher);

        var book = new Book
        {
            Id = bookId,
            Title = "Test Kitap",
            ISBN = "123",
            Stock = 5,
            PublisherId = publisher.Id
        };
        _context.Books.Add(book);

        var borrowing = new Borrowing
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = bookId,
            Status = BorrowingStatus.Returned, // zaten iade edilmiş
            ReturnedAt = DateTime.UtcNow
        };
        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();

        // Act & Assert
        var act = async () => await _borrowingService.ReturnAsync(borrowing.Id);
        await act.Should().ThrowAsync<AlreadyReturnedException>();
    }
}