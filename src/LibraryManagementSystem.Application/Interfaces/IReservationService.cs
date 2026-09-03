using LibraryManagementSystem.Application.DTOs.Reservation;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> CreateAsync(Guid userId, CreateReservationRequest request);
    Task<List<ReservationDto>> GetAllAsync(Guid userId);
}