namespace LibraryManagementSystem.Application.DTOs.Book;

public class CreateBookRequest
{
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public Guid PublisherId { get; set; }
    public int Stock { get; set; }
    public int? PublishedYear { get; set; }
    public string? Description { get; set; }

    // Kitap oluşturulurken birden fazla yazar ve kategori atanabilmeli
    public List<Guid> AuthorIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();
}