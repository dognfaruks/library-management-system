using LibraryManagementSystem.Application.DTOs.Review;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateAsync(Guid userId, CreateReviewRequest request);
    Task<List<ReviewDto>> GetAllAsync(Guid? bookId);
}