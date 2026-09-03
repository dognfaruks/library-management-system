namespace LibraryManagementSystem.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // USER, ADMIN

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}