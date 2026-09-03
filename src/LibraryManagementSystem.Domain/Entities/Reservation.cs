using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    public int QueueOrder { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
}