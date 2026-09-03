namespace LibraryManagementSystem.Application.DTOs.Review;

public class CreateReviewRequest
{
    public Guid BookId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}