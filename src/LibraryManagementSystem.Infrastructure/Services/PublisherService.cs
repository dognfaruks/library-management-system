using LibraryManagementSystem.Application.DTOs.Publisher;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Services;

public class PublisherService : IPublisherService
{
    private readonly LibraryDbContext _context;

    public PublisherService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<PublisherDto>> GetAllAsync()
    {
        return await _context.Publishers
            .Select(p => new PublisherDto { Id = p.Id, Name = p.Name })
            .ToListAsync();
    }

    public async Task<PublisherDto?> GetByIdAsync(Guid id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null) return null;

        return new PublisherDto { Id = publisher.Id, Name = publisher.Name };
    }

    public async Task<PublisherDto> CreateAsync(CreatePublisherRequest request)
    {
        var publisher = new Domain.Entities.Publisher { Name = request.Name };

        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();

        return new PublisherDto { Id = publisher.Id, Name = publisher.Name };
    }

    public async Task<PublisherDto?> UpdateAsync(Guid id, UpdatePublisherRequest request)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null) return null;

        publisher.Name = request.Name;
        await _context.SaveChangesAsync();

        return new PublisherDto { Id = publisher.Id, Name = publisher.Name };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher is null) return false;

        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();
        return true;
    }
}