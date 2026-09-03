using LibraryManagementSystem.Application.DTOs.Borrowing;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IBorrowingService
{
    Task<BorrowingDto> CreateAsync(Guid userId, CreateBorrowingRequest request);
    Task<BorrowingDto> ReturnAsync(Guid borrowingId);
    Task<List<BorrowingDto>> GetAllAsync();
}