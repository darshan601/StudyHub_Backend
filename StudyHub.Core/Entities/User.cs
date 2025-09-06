namespace StudyHub.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "student"; //student or admin
}