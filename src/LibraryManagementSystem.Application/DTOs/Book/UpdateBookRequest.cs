namespace LibraryManagementSystem.Application.DTOs.Book;

public class UpdateBookRequest
{
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public Guid PublisherId { get; set; }
    public int Stock { get; set; }
    public int? PublishedYear { get; set; }
    public string? Description { get; set; }
    public List<Guid> AuthorIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();
}