using LibraryManagementSystem.Application.DTOs.Author;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<AuthorDto?> GetByIdAsync(Guid id);
    Task<AuthorDto> CreateAsync(CreateAuthorRequest request);
    Task<AuthorDto?> UpdateAsync(Guid id, UpdateAuthorRequest request);
    Task<bool> DeleteAsync(Guid id);
}