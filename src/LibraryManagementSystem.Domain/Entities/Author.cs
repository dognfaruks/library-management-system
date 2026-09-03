namespace LibraryManagementSystem.Domain.Entities;

public class Author
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = null!;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}