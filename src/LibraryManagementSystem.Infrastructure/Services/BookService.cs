using LibraryManagementSystem.Application.DTOs.Book;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly LibraryDbContext _context;

    public BookService(LibraryDbContext context)
    {
        _context = context;
    }

        public async Task<PagedResult<BookDto>> GetAllAsync(BookQueryParameters parameters)
    {
        var query = _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .AsQueryable();

        // ---------- Arama (başlıkta, case-insensitive) ----------
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            query = query.Where(b => b.Title.ToLower().Contains(parameters.Search.ToLower()));
        }

        // ---------- Filtreleme ----------
        if (!string.IsNullOrWhiteSpace(parameters.Category))
        {
            query = query.Where(b => b.BookCategories.Any(bc =>
                bc.Category.Name.ToLower() == parameters.Category.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Author))
        {
            query = query.Where(b => b.BookAuthors.Any(ba =>
                ba.Author.FullName.ToLower().Contains(parameters.Author.ToLower())));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Publisher))
        {
            query = query.Where(b => b.Publisher.Name.ToLower().Contains(parameters.Publisher.ToLower()));
        }

        // ---------- Sıralama ----------
        query = parameters.SortBy?.ToLower() switch
        {
            "title" => query.OrderBy(b => b.Title),
            "stock" => query.OrderBy(b => b.Stock),
            "publishedyear" => query.OrderBy(b => b.PublishedYear),
            _ => query.OrderBy(b => b.Title) // varsayılan sıralama
        };

        // ---------- Sayfalama ----------
        var totalCount = await query.CountAsync();

        var page = parameters.Page < 1 ? 1 : parameters.Page;
        var limit = parameters.Limit < 1 ? 10 : parameters.Limit;

        var books = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return new PagedResult<BookDto>
        {
            Items = books.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            Limit = limit,
            TotalPages = (int)Math.Ceiling(totalCount / (double)limit)
        };
    }

    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        return book is null ? null : MapToDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            ISBN = request.ISBN,
            PublisherId = request.PublisherId,
            Stock = request.Stock,
            PublishedYear = request.PublishedYear,
            Description = request.Description
        };

        // Çoklu yazar ataması
        foreach (var authorId in request.AuthorIds)
        {
            book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
        }

        // Çoklu kategori ataması
        foreach (var categoryId in request.CategoryIds)
        {
            book.BookCategories.Add(new BookCategory { CategoryId = categoryId });
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Yeni oluşturulan kitabı, ilişkili verilerle (Publisher, Authors, Categories) tekrar çekiyoruz
        return (await GetByIdAsync(book.Id))!;
    }

    public async Task<BookDto?> UpdateAsync(Guid id, UpdateBookRequest request)
    {
        var book = await _context.Books
            .Include(b => b.BookAuthors)
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null) return null;

        book.Title = request.Title;
        book.ISBN = request.ISBN;
        book.PublisherId = request.PublisherId;
        book.Stock = request.Stock;
        book.PublishedYear = request.PublishedYear;
        book.Description = request.Description;

        // İlişkileri sıfırdan kur: önce mevcut ilişkileri tamamen temizle...
        book.BookAuthors.Clear();
        book.BookCategories.Clear();

        // ...sonra istekte gelen yeni listeyle tekrar doldur
        foreach (var authorId in request.AuthorIds)
        {
            book.BookAuthors.Add(new BookAuthor { BookId = id, AuthorId = authorId });
        }

        foreach (var categoryId in request.CategoryIds)
        {
            book.BookCategories.Add(new BookCategory { BookId = id, CategoryId = categoryId });
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Stock = book.Stock,
            PublishedYear = book.PublishedYear,
            Description = book.Description,
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher.Name,
            Authors = book.BookAuthors.Select(ba => new AuthorSummaryDto
            {
                Id = ba.Author.Id,
                FullName = ba.Author.FullName
            }).ToList(),
            Categories = book.BookCategories.Select(bc => new CategorySummaryDto
            {
                Id = bc.Category.Id,
                Name = bc.Category.Name
            }).ToList()
        };
    }
}