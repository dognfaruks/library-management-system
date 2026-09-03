using LibraryManagementSystem.Application.DTOs.Auth;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly LibraryDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private const string DefaultRoleName = "USER";

        private readonly ILogger<AuthService> _logger;

    public AuthService(
        LibraryDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // İş kuralı: Email zaten kayıtlıysa 409 Conflict dönecek şekilde işaretle
        var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
        {
            throw new DuplicateEmailException(request.Email);
        }

        // Şifreyi asla düz metin saklamıyoruz - bcrypt ile hashliyoruz
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        // Varsayılan rol: USER
        var defaultRole = await _context.Roles.FirstAsync(r => r.Name == DefaultRoleName);
        user.UserRoles.Add(new UserRole { User = user, Role = defaultRole });

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var roles = new List<string> { DefaultRoleName };
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roles);
        
        _logger.LogInformation("Yeni kullanıcı kaydı: {Username} ({Email})", user.Username, user.Email);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Roles = roles,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Kullanıcı yoksa veya şifre yanlışsa aynı hatayı dönüyoruz
        // (güvenlik: "email yok" ile "şifre yanlış" ayrımını dışarı sızdırmıyoruz)
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roles);
        
        _logger.LogInformation("Kullanıcı girişi: {Username} ({Email})", user.Username, user.Email);
        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Roles = roles,
            ExpiresAt = expiresAt
        };
    }
}