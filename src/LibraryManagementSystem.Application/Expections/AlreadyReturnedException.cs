namespace LibraryManagementSystem.Application.Exceptions;

public class AlreadyReturnedException : Exception
{
    public AlreadyReturnedException()
        : base("Bu kitap zaten iade edilmiş.")
    {
    }
}