namespace LibraryManagementSystem.Application.DTOs.Book;

public class BookQueryParameters
{
    // Sayfalama
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;

    // Arama (başlıkta arama yapar)
    public string? Search { get; set; }

    // Filtreleme
    public string? Category { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }

    // Sıralama
    public string? SortBy { get; set; }
}