namespace LibraryManagementSystem.Application.DTOs.Reservation;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public DateTime ReservedAt { get; set; }
    public int QueueOrder { get; set; }
    public string Status { get; set; } = null!;
}