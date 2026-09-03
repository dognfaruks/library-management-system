namespace LibraryManagementSystem.Application.DTOs.Borrowing;

public class BorrowingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string Status { get; set; } = null!;
}