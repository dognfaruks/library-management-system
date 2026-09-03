namespace LibraryManagementSystem.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}