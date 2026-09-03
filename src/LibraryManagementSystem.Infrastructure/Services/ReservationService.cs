using LibraryManagementSystem.Application.DTOs.Reservation;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Infrastructure.Services;

public class ReservationService : IReservationService
{
    private readonly LibraryDbContext _context;

    private readonly ILogger<ReservationService> _logger;

    public ReservationService(LibraryDbContext context, ILogger<ReservationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReservationDto> CreateAsync(Guid userId, CreateReservationRequest request)
    {
        var book = await _context.Books.FindAsync(request.BookId);
        if (book is null)
        {
            throw new NotFoundException("Kitap bulunamadı.");
        }

        // İş kuralı: Aynı kullanıcı aynı kitap için tekrar AKTİF rezervasyon oluşturamaz
        // (Veritabanındaki partial unique index bunu zaten garanti ediyor,
        //  ama burada kontrol ederek kullanıcıya daha anlamlı bir hata mesajı veriyoruz)
        var alreadyExists = await _context.Reservations.AnyAsync(r =>
            r.UserId == userId &&
            r.BookId == request.BookId &&
            r.Status == ReservationStatus.Active);

        if (alreadyExists)
        {
            throw new DuplicateReservationException(book.Title);
        }

        // Sıra numarasını belirle: bu kitap için en son sıradan bir fazlası
        var currentMaxQueue = await _context.Reservations
            .Where(r => r.BookId == request.BookId && r.Status == ReservationStatus.Active)
            .Select(r => (int?)r.QueueOrder)
            .MaxAsync() ?? 0;

        var reservation = new Reservation
        {
            UserId = userId,
            BookId = request.BookId,
            ReservedAt = DateTime.UtcNow,
            QueueOrder = currentMaxQueue + 1,
            Status = ReservationStatus.Active
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Rezervasyon oluşturuldu: Kullanıcı {UserId}, Kitap '{BookTitle}'", userId, book.Title);
        
        return await MapToDtoAsync(reservation.Id);
    }

    public async Task<List<ReservationDto>> GetAllAsync(Guid userId)
    {
        var reservations = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Book)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.QueueOrder)
            .ToListAsync();

        return reservations.Select(MapToDto).ToList();
    }

    private async Task<ReservationDto> MapToDtoAsync(Guid id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Book)
            .FirstAsync(r => r.Id == id);

        return MapToDto(reservation);
    }

    private static ReservationDto MapToDto(Reservation r)
    {
        return new ReservationDto
        {
            Id = r.Id,
            UserId = r.UserId,
            Username = r.User.Username,
            BookId = r.BookId,
            BookTitle = r.Book.Title,
            ReservedAt = r.ReservedAt,
            QueueOrder = r.QueueOrder,
            Status = r.Status.ToString()
        };
    }
}