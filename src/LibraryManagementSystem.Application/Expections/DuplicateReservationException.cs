namespace LibraryManagementSystem.Application.Exceptions;

public class DuplicateReservationException : Exception
{
    public DuplicateReservationException(string bookTitle)
        : base($"'{bookTitle}' kitabı için zaten aktif bir rezervasyonunuz var.")
    {
    }
}