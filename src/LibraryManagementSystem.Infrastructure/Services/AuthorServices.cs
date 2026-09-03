using LibraryManagementSystem.Application.DTOs.Author;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly LibraryDbContext _context;

    public AuthorService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        return await _context.Authors
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                FullName = a.FullName
            })
            .ToListAsync();
    }

    public async Task<AuthorDto?> GetByIdAsync(Guid id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null) return null;

        return new AuthorDto
        {
            Id = author.Id,
            FullName = author.FullName
        };
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorRequest request)
    {
        var author = new Domain.Entities.Author
        {
            FullName = request.FullName
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        return new AuthorDto
        {
            Id = author.Id,
            FullName = author.FullName
        };
    }

    public async Task<AuthorDto?> UpdateAsync(Guid id, UpdateAuthorRequest request)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null) return null;

        author.FullName = request.FullName;
        await _context.SaveChangesAsync();

        return new AuthorDto
        {
            Id = author.Id,
            FullName = author.FullName
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null) return false;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return true;
    }
}