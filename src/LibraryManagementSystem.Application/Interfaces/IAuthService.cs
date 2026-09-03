using LibraryManagementSystem.Application.DTOs.Auth;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}