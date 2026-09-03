namespace LibraryManagementSystem.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}