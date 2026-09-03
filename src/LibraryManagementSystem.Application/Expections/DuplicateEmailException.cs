namespace LibraryManagementSystem.Application.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"'{email}' e-posta adresi zaten kayıtlı.")
    {
    }
}