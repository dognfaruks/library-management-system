namespace LibraryManagementSystem.Domain.Entities;

// Composite key: (UserId, RoleId) - ileride DbContext içinde tanımlanacak
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}