namespace LibraryManagementSystem.Domain.Entities;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;

    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public int Stock { get; set; }
    public int? PublishedYear { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}