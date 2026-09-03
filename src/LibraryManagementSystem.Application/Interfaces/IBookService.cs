using LibraryManagementSystem.Application.DTOs.Book;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IBookService
{
    Task<PagedResult<BookDto>> GetAllAsync(BookQueryParameters parameters);    Task<BookDto?> GetByIdAsync(Guid id);
    Task<BookDto> CreateAsync(CreateBookRequest request);
    Task<BookDto?> UpdateAsync(Guid id, UpdateBookRequest request);
    Task<bool> DeleteAsync(Guid id);
}