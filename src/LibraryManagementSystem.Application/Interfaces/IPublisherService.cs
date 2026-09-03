using LibraryManagementSystem.Application.DTOs.Publisher;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IPublisherService
{
    Task<List<PublisherDto>> GetAllAsync();
    Task<PublisherDto?> GetByIdAsync(Guid id);
    Task<PublisherDto> CreateAsync(CreatePublisherRequest request);
    Task<PublisherDto?> UpdateAsync(Guid id, UpdatePublisherRequest request);
    Task<bool> DeleteAsync(Guid id);
}