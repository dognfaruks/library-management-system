namespace LibraryManagementSystem.Application.Exceptions;

public class DuplicateReviewException : Exception
{
    public DuplicateReviewException(string bookTitle)
        : base($"'{bookTitle}' kitabına zaten bir yorum yaptınız.")
    {
    }
}