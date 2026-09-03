namespace LibraryManagementSystem.Domain.Entities;

public class Publisher
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}