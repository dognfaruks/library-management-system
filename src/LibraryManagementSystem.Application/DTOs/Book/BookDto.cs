namespace LibraryManagementSystem.Application.DTOs.Book;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int Stock { get; set; }
    public int? PublishedYear { get; set; }
    public string? Description { get; set; }

    public Guid PublisherId { get; set; }
    public string PublisherName { get; set; } = null!;

    public List<AuthorSummaryDto> Authors { get; set; } = new();
    public List<CategorySummaryDto> Categories { get; set; } = new();
}

public class AuthorSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
}

public class CategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}