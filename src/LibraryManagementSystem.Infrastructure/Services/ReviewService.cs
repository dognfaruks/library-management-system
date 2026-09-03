using LibraryManagementSystem.Application.DTOs.Review;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly LibraryDbContext _context;

    public ReviewService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewDto> CreateAsync(Guid userId, CreateReviewRequest request)
    {
        // İş kuralı: Puan 1-5 arasında olmalı
        if (request.Rating is < 1 or > 5)
        {
            throw new InvalidRatingException();
        }

        var book = await _context.Books.FindAsync(request.BookId);
        if (book is null)
        {
            throw new NotFoundException("Kitap bulunamadı.");
        }

        // İş kuralı: Bir kullanıcı bir kitaba yalnızca bir kez yorum yapabilir
        var alreadyReviewed = await _context.Reviews.AnyAsync(r =>
            r.UserId == userId && r.BookId == request.BookId);

        if (alreadyReviewed)
        {
            throw new DuplicateReviewException(book.Title);
        }

        var review = new Review
        {
            UserId = userId,
            BookId = request.BookId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return await MapToDtoAsync(review.Id);
    }

    public async Task<List<ReviewDto>> GetAllAsync(Guid? bookId)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .AsQueryable();

        if (bookId.HasValue)
        {
            query = query.Where(r => r.BookId == bookId.Value);
        }

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(MapToDto).ToList();
    }

    private async Task<ReviewDto> MapToDtoAsync(Guid id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .FirstAsync(r => r.Id == id);

        return MapToDto(review);
    }

    private static ReviewDto MapToDto(Review r)
    {
        return new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            Username = r.User.Username,
            BookId = r.BookId,
            BookTitle = r.Book.Title,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}