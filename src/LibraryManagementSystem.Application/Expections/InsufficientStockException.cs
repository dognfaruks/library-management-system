namespace LibraryManagementSystem.Application.Exceptions;

public class InsufficientStockException : Exception
{
    public InsufficientStockException(string bookTitle)
        : base($"'{bookTitle}' kitabının stoğu yok. Rezervasyon oluşturabilirsiniz.")
    {
    }
}