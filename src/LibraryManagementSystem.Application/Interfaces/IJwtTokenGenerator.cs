using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user, IList<string> roles);
}