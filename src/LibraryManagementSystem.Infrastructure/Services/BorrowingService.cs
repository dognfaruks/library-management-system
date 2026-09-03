using LibraryManagementSystem.Application.DTOs.Borrowing;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Infrastructure.Services;

public class BorrowingService : IBorrowingService
{
    private readonly LibraryDbContext _context;

        private readonly ILogger<BorrowingService> _logger;

    public BorrowingService(LibraryDbContext context, ILogger<BorrowingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BorrowingDto> CreateAsync(Guid userId, CreateBorrowingRequest request)
    {
        var book = await _context.Books.FindAsync(request.BookId);
        if (book is null)
        {
            throw new NotFoundException("Kitap bulunamadı.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        // KRİTİK SATIR: Stok azaltma işlemini atomik (bölünemez) bir SQL komutu olarak yapıyoruz.
        // "WHERE Stock > 0" koşulu sayesinde, iki istek aynı anda gelse bile
        // veritabanı bu satırı aynı anda sadece BİRİNE güncelletir (row-level lock otomatik uygulanır).
        // Eğer stok zaten 0'sa, hiçbir satır güncellenmez ve rowsAffected = 0 döner.
        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Books\" SET \"Stock\" = \"Stock\" - 1 WHERE \"Id\" = {request.BookId} AND \"Stock\" > 0");

        if (rowsAffected == 0)
        {
            await transaction.RollbackAsync();
            throw new InsufficientStockException(book.Title);
        }

        var borrowing = new Borrowing
        {
            UserId = userId,
            BookId = request.BookId,
            BorrowedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = BorrowingStatus.Active
        };

        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();

        // İkisi de (stok azaltma + borrowing kaydı) başarılıysa, kalıcı hale getir
        await transaction.CommitAsync();
        
        _logger.LogInformation("Ödünç alma: Kullanıcı {UserId}, Kitap '{BookTitle}'", userId, book.Title);
        return await MapToDtoAsync(borrowing.Id);
    }

    public async Task<BorrowingDto> ReturnAsync(Guid borrowingId)
    {
        var borrowing = await _context.Borrowings.FindAsync(borrowingId);
        if (borrowing is null)
        {
            throw new NotFoundException("Ödünç kaydı bulunamadı.");
        }

        if (borrowing.Status == BorrowingStatus.Returned)
        {
            throw new AlreadyReturnedException();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        borrowing.Status = BorrowingStatus.Returned;
        borrowing.ReturnedAt = DateTime.UtcNow;

        // Stoğu geri artır (aynı transaction içinde)
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Books\" SET \"Stock\" = \"Stock\" + 1 WHERE \"Id\" = {borrowing.BookId}");

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return await MapToDtoAsync(borrowing.Id);
    }

    public async Task<List<BorrowingDto>> GetAllAsync()
    {
        var borrowings = await _context.Borrowings
            .Include(b => b.User)
            .Include(b => b.Book)
            .OrderByDescending(b => b.BorrowedAt)
            .ToListAsync();

        return borrowings.Select(MapToDto).ToList();
    }

    private async Task<BorrowingDto> MapToDtoAsync(Guid id)
    {
        var borrowing = await _context.Borrowings
            .Include(b => b.User)
            .Include(b => b.Book)
            .FirstAsync(b => b.Id == id);

        return MapToDto(borrowing);
    }

    private static BorrowingDto MapToDto(Borrowing b)
    {
        return new BorrowingDto
        {
            Id = b.Id,
            UserId = b.UserId,
            Username = b.User.Username,
            BookId = b.BookId,
            BookTitle = b.Book.Title,
            BorrowedAt = b.BorrowedAt,
            DueDate = b.DueDate,
            ReturnedAt = b.ReturnedAt,
            Status = b.Status.ToString()
        };
    }
}