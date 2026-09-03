namespace LibraryManagementSystem.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}