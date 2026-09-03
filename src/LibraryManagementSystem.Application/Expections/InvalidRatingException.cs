namespace LibraryManagementSystem.Application.Exceptions;

public class InvalidRatingException : Exception
{
    public InvalidRatingException()
        : base("Puan 1 ile 5 arasında olmalıdır.")
    {
    }
}