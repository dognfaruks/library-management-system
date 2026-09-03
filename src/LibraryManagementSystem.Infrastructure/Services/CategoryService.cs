using LibraryManagementSystem.Application.DTOs.Category;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly LibraryDbContext _context;

    public CategoryService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return null;

        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Domain.Entities.Category { Name = request.Name };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return null;

        category.Name = request.Name;
        await _context.SaveChangesAsync();

        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}